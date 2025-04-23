#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEditor;

namespace UiProjector.Sandbox
{
    /// <summary>
    /// 3Dオブジェクトに追従するUIを生成する
    /// </summary>
    public class ProjectionUiEmitter : MonoBehaviour
    {
        private enum CanvasRenderMode
        {
            Overlay,
            CameraSpace
        }

        private enum ReviseMode
        {
            None,
            Screen,
            SafeArea
        }

        [SerializeField] private ObjectProvider _objectProvider;
        [Space]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uiCamera;
        [Space]
        [SerializeField] private Canvas _originalCanvasOverlay;
        [SerializeField] private Canvas _originalCanvasCameraSpace;
        [SerializeField] private CanvasRenderMode _renderMode;
        [Space]
        [SerializeField] private RectTransform _uiPrefab;
        [SerializeField] private Color _uiColor;
        [Space]
        [SerializeField] private ReviseMode _reviseMode;
        [SerializeField] private Vector3 _worldSpaceOffset;
        [SerializeField] private Vector2 _screenSpaceOffset;
        [SerializeField] private bool _useObjectPool;

        private ObjectPool<RectTransform> _uiPool;
        private IProjectionReviser _reviser;

        private CanvasHandle _canvasHandle;
        private Stack<UiHandle> _uiHandles = new();

        private void Awake()
        {
            // オブジェクトプールを初期化
            if (_useObjectPool)
            {
                _uiPool = new ObjectPool<RectTransform>(
                    // create
                    () => Instantiate(_uiPrefab, transform),
                    // get
                    ui => ui.gameObject.SetActive(true),
                    // release
                    ui =>
                    {
                        ui.gameObject.SetActive(false);
                        ui.transform.SetParent(transform);
                    });
            }

            // Reviserを作成
            switch (_reviseMode)
            {
                case ReviseMode.None:
                    _reviser = null;
                    break;
                case ReviseMode.Screen:
                    _reviser = new ScreenReviser();
                    break;
                case ReviseMode.SafeArea:
                    _reviser = new SafeAreaReviser();
                    break;
            }

            // Canvasを作成
            var canvas = _renderMode == CanvasRenderMode.Overlay ? _originalCanvasOverlay : _originalCanvasCameraSpace;
            var camera = _renderMode == CanvasRenderMode.Overlay ? _mainCamera : _uiCamera;
            // 生成するCanvasのコピー元となるオリジナルと、UIを描画するCameraを指定する
            _canvasHandle = ProjectionUi.CreateCanvas(canvas, camera);
        }

        /// <summary>
        /// ProjectionUIを追加する
        /// </summary>
        public void Emit()
        {
            // UIのセットアップ
            var ui = _useObjectPool ? _uiPool.Get() : Instantiate(_uiPrefab, transform);
            ui.GetComponent<Image>().color = _uiColor;

            // ProjectionUIを追加する
            var uiHandle = _canvasHandle.AddUi(
                // 追加するUI
                ui,
                // 追従対象オブジェクトのTransform
                _objectProvider.GetRandom().transform,
                // ワールド空間でのオフセット
                _worldSpaceOffset,
                // スクリーン空間でのオフセット
                _screenSpaceOffset,
                // UIの位置を修正するためのIProjectionReviser
                _reviser,
                // 解放時の処理
                _useObjectPool ? (x => _uiPool.Release(x)) : Destroy);

            // UiHandleをスタックに追加
            _uiHandles.Push(uiHandle);
        }

        /// <summary>
        /// ProjectionUIを解放する
        /// </summary>
        public void Release()
        {
            // スタックからUiHandleを取り出して解放
            if (_uiHandles.Count > 0)
            {
                _uiHandles.Pop().Release();
            }
        }

        private void Oestroy()
        {
            _canvasHandle.Dispose();
            _uiPool.Dispose();
        }
    }

    [CustomEditor(typeof(ProjectionUiEmitter))]
    public class ProjectionUiEmitterEditor : Editor
    {
        private ProjectionUiEmitter _emitter;

        private void OnEnable()
        {
            _emitter = (ProjectionUiEmitter)target;
        }

        public override void OnInspectorGUI()
        {
            // 実行環境の間はディセーブル
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            // ボタンは実行環境でのみ使用可能
            if (!Application.isPlaying) return;
            if (GUILayout.Button("Emit")) _emitter.Emit();
            if (GUILayout.Button("Release")) _emitter.Release();
        }
    }
}
#endif