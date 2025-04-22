using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UiProjector.Internal
{
    internal interface IProjectionStrategy
    {
        /// <summary> ターゲットをスクリーンへ投影した場所にUIを移動させる </summary>
        void Project(Binding binding);
    }

    internal class Binding
    {
        public RectTransform Ui;
        public Transform Target;
        public Vector3 WorldSpaceOffset;
        public Vector2 ScreenSpaceOffset;
        public IProjectionReviser Reviser;
        public Action<RectTransform> ReleaseAction;
    }

    /// <summary>
    /// 配下のUIを管理し、対応オブジェクトをスクリーンに投影した位置に動かすCanvas
    /// </summary>
    internal class ProjectionUiCanvas : MonoBehaviour
    {
        private Canvas _canvas;
        private Camera _camera;
        private Dictionary<UiId, Binding> _bindingMap = new();
        private IProjectionStrategy _projector;
#if UNITY_EDITOR
        private RenderMode _lastRenderMode;
#endif

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Canvas canvas, Camera camera)
        {
            _canvas = canvas;
            _camera = camera ?? Camera.main;
            _projector = _canvas.renderMode switch
            {
                RenderMode.ScreenSpaceOverlay => new OverlayProjectionStrategy(_camera),
                RenderMode.ScreenSpaceCamera => new CameraSpaceProjectionStrategy(_camera),
                RenderMode.WorldSpace => throw new NotSupportedException($"RenderMode.WorldSpace はサポート外です。Canvas: {_canvas.name}"),
                _ => throw new ArgumentOutOfRangeException()
            };
#if UNITY_EDITOR
            _lastRenderMode = _canvas.renderMode;
#endif
        }

#if UNITY_EDITOR
        private void Reset()
        {
            // インスペクタからアタッチ禁止
            EditorApplication.delayCall += () => DestroyImmediate(this);
        }

        private void Update()
        {
            // RenderModeが変わったら再初期化
            if (_lastRenderMode != _canvas.renderMode)
            {
                _lastRenderMode = _canvas.renderMode;
                Initialize(_canvas, _camera);
            }
        }
#endif
        private void LateUpdate()
        {
            foreach (var binding in _bindingMap.Values)
            {
                _projector.Project(binding);
            }
        }

        /// <summary>
        /// UIを追加して管理下に加える
        /// </summary>
        public void Add(UiId id, RectTransform ui, Transform target, Vector3 worldSpaceOffset, Vector2 screenSpaceOffset, IProjectionReviser reviser, Action<RectTransform> releaseAction)
        {
            // スケールを保持
            var scale = ui.localScale;
            // 子にする
            ui.transform.SetParent(_canvas.transform);
            // スケールを戻す
            ui.transform.localScale = scale;

            // Bindingを追加
            var binding = new Binding
            {
                Target = target,
                Ui = ui,
                WorldSpaceOffset = worldSpaceOffset,
                ScreenSpaceOffset = screenSpaceOffset,
                Reviser = reviser,
                ReleaseAction = releaseAction
            };
            _bindingMap[id] = binding;
        }

        /// <summary>
        /// UIを解放する
        /// </summary>
        public void ReleaseUi(UiId id)
        {
            if (!_bindingMap.ContainsKey(id))
            {
                Debug.LogWarning($"存在しないUIを解放しようとしました。ID: {id}");
                return;
            }

            var binding = _bindingMap[id];
            _bindingMap.Remove(id);
            binding.ReleaseAction(binding.Ui);
        }

        /// <summary>
        /// UIを全て解放する
        /// </summary>
        public void Clear()
        {
            foreach (var uiId in _bindingMap.Keys)
            {
                // このClearメソッドでは警告が出ないようにしたいのでcontinue
                if (!_bindingMap.ContainsKey(uiId)) continue;

                ReleaseUi(uiId);
            }

            _bindingMap.Clear();
        }

        /// <summary>
        /// 現在も有効なUIであるかどうか
        /// </summary>
        public bool IsValidUi(UiId id)
        {
            return _bindingMap.ContainsKey(id);
        }
    }
}
