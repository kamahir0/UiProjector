using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UiProjector.Internal
{
    /// <summary>
    /// ProjectionUiを管理する
    /// </summary>
    internal class ProjectionUiManager
    {
        /// <summary> シングルトン </summary>
        public static ProjectionUiManager Instance { get; } = new ProjectionUiManager();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            // DontDestroyOnLoadする
            var canvasRoot = new GameObject("ProjectionUi");
            Instance._canvasRoot = canvasRoot.transform;
            Object.DontDestroyOnLoad(canvasRoot);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void LateInitialize()
        {
            // デフォルトCanvasを作成
            var obj = new GameObject("Default");
            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            var scaler = obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            var raycaster = obj.AddComponent<GraphicRaycaster>();
            var canvasId = Instance.CreateCanvas(canvas, Camera.main);

            // コピーが作成されたので元は削除
            Object.Destroy(obj);

            Instance.DefaultCanvasId = canvasId;
            Instance.DefaultCanvas = canvas;
            Instance.DefaultCanvasScaler = scaler;
            Instance.DefaultCanvasRaycaster = raycaster;
        }

        /// <summary> デフォルトCanvasのID </summary>
        public CanvasId DefaultCanvasId { get; private set; }
        /// <summary> デフォルトCanvas </summary>
        public Canvas DefaultCanvas { get; private set; }
        /// <summary> デフォルトCanvasのCanvasScaler </summary>
        public CanvasScaler DefaultCanvasScaler { get; private set; }
        /// <summary> デフォルトCanvasのGraphicRaycaster </summary>
        public GraphicRaycaster DefaultCanvasRaycaster { get; private set; }

        private Transform _canvasRoot;
        private Dictionary<CanvasId, ProjectionUiCanvas> _canvasMap = new();
        private int _canvasIdCounter;
        private int _uiIdCounter;

        /// <summary>
        /// Canvasを作成する
        /// </summary>
        public CanvasId CreateCanvas(Canvas original, Camera camera)
        {
            // Canvasを複製
            var duplicated = Object.Instantiate(original, original.transform.parent);
            duplicated.name = original.name;
            duplicated.gameObject.SetActive(true);
            duplicated.transform.SetParent(_canvasRoot);

            // 子が存在していたら削除
            foreach (Transform child in duplicated.transform)
            {
                Object.Destroy(child.gameObject);
            }

            // ProjectionUiCanvasを追加
            var canvas = duplicated.gameObject.AddComponent<ProjectionUiCanvas>();
            canvas.Initialize(duplicated, camera);

            // IDを割り当てる
            var canvasId = new CanvasId(_canvasIdCounter++);
            if (!_canvasMap.ContainsKey(canvasId))
            {
                _canvasMap[canvasId] = canvas;
            }

            return canvasId;
        }

        /// <summary>
        /// UIを追加する
        /// </summary>
        public UiId AddUi(CanvasId canvasId, RectTransform ui, Transform target, Vector3 worldSpaceOffset, Vector2 screenSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null)
        {
            // Reviserが指定されていない場合は何も修正をかけない
            reviser ??= IdentityReviser.Instance;

            // Release時の挙動が指定されていない場合はDestroyする
            releaseAction ??= ui => Object.Destroy(ui.gameObject);

            // IDを割り当てる
            var uiId = new UiId(_uiIdCounter++);

            // UIを追加
            var canvas = _canvasMap[canvasId];
            canvas.Add(uiId, ui, target, worldSpaceOffset, screenSpaceOffset, reviser, releaseAction);

            return uiId;
        }

        /// <summary>
        /// UIを解放する
        /// </summary>
        public void ReleaseUi(CanvasId canvasId, UiId uiId)
        {
            var canvas = _canvasMap[canvasId];
            canvas.ReleaseUi(uiId);
        }

        /// <summary>
        /// Canvas配下のUIを全て解放する
        /// </summary>
        public void ClearCanvas(CanvasId canvasId)
        {
            _canvasMap[canvasId].Clear();
        }

        /// <summary>
        /// Canvasを破棄する
        /// </summary>
        public void DisposeCanvas(CanvasId canvasId)
        {
            var canvas = _canvasMap[canvasId];
            _canvasMap.Remove(canvasId);
            canvas.Clear();
            Object.Destroy(canvas.gameObject);
        }

        /// <summary>
        /// 現在も有効なCanvasであるかどうか
        /// </summary>
        public bool IsValidCanvas(CanvasId canvasId)
        {
            return _canvasMap.ContainsKey(canvasId);
        }

        /// <summary>
        /// 現在も有効なUIであるかどうか
        /// </summary>
        public bool IsValidUi(CanvasId canvasId, UiId uiId)
        {
            if (!_canvasMap.ContainsKey(canvasId)) return false;

            var canvas = _canvasMap[canvasId];
            return canvas.IsValidUi(uiId);
        }

        /// <summary>
        /// CanvasのSortingOrderを取得する
        /// </summary>
        public int GetSortingOrder(CanvasId canvasId)
        {
            var canvas = _canvasMap[canvasId];
            return canvas.SortingOrder;
        }

        /// <summary>
        /// CanvasのSortingOrderを変更する
        /// </summary>
        public void SetSortingOrder(CanvasId canvasId, int order)
        {
            var canvas = _canvasMap[canvasId];
            canvas.SortingOrder = order;
        }
    }
}