using System;
using UnityEngine;
using UnityEngine.UI;
using UiProjector.Internal;

namespace UiProjector
{
    /// <summary>
    /// 各種機能を呼び出すための静的クラス
    /// </summary>
    public static class ProjectionUi
    {
        /// <summary>
        /// Canvasを作成する
        /// </summary>
        public static CanvasHandle CreateCanvas(Canvas original, Camera camera)
        {
            var id = ProjectionUiManager.Instance.CreateCanvas(original, camera);
            return new CanvasHandle(id);
        }

        // デフォルトCanvasのハンドラ
        //NOTE: 公開するとDisposeできてしまうのでこいつは直接触れないようにする
        private static CanvasHandle DefaultCanvasHandle => _defaultCanvasHandle ??= new CanvasHandle(ProjectionUiManager.Instance.DefaultCanvasId);
        private static CanvasHandle? _defaultCanvasHandle;

        /// <summary> デフォルトCanvas </summary>
        public static Canvas DefaultCanvas => ProjectionUiManager.Instance.DefaultCanvas;

        /// <summary> デフォルトCanvasのCanvasScaler </summary>
        public static CanvasScaler DefaultCanvasScaler => ProjectionUiManager.Instance.DefaultCanvasScaler;

        /// <summary> デフォルトCanvasのGraphicRaycaster </summary>
        public static GraphicRaycaster DefaultCanvasRaycaster => ProjectionUiManager.Instance.DefaultCanvasRaycaster;

        /// <summary>
        /// UIを追加する。デフォルトCanvasを使用する
        /// </summary>
        /// <param name="ui">追従させるUIのRectTransform</param>
        /// <param name="target">追従対象のTransform</param>
        /// <param name="worldSpaceOffset">ワールド空間でのオフセット</param>
        /// <param name="screenSpaceOffset">スクリーン空間でのオフセット</param>
        /// <param name="reviser">UIの位置を修正するProjectionReviser</param>
        /// <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public static UiHandle AddUi(RectTransform ui, Transform target, Vector3 worldSpaceOffset, Vector2 screenSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null)
        {
            return DefaultCanvasHandle.Add(ui, target, worldSpaceOffset, screenSpaceOffset, reviser, releaseAction);
        }
        /// <summary> UIを追加する。デフォルトCanvasを使用する </summary> <param name="ui">追従させるUIのRectTransform</param> <param name="target">追従対象のTransform</param> <param name="worldSpaceOffset">ワールド空間でのオフセット</param> <param name="reviser">UIの位置を修正するProjectionReviser</param> <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public static UiHandle AddUi(RectTransform ui, Transform target, Vector3 worldSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null) => AddUi(ui, target, worldSpaceOffset, Vector2.zero, reviser, releaseAction);

        /// <summary> UIを追加する。デフォルトCanvasを使用する </summary> <param name="ui">追従させるUIのRectTransform</param> <param name="target">追従対象のTransform</param> <param name="screenSpaceOffset">スクリーン空間でのオフセット</param> <param name="reviser">UIの位置を修正するProjectionReviser</param> <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public static UiHandle AddUi(RectTransform ui, Transform target, Vector2 screenSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null) => AddUi(ui, target, Vector3.zero, screenSpaceOffset, reviser, releaseAction);

        /// <summary> UIを追加する。デフォルトCanvasを使用する </summary> <param name="ui">追従させるUIのRectTransform</param> <param name="target">追従対象のTransform</param> <param name="reviser">UIの位置を修正するProjectionReviser</param> <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public static UiHandle AddUi(RectTransform ui, Transform target, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null) => AddUi(ui, target, Vector3.zero, Vector2.zero, reviser, releaseAction);

        /// <summary>
        /// デフォルトCanvas配下のUIを全て解放する
        /// </summary>
        public static void ClearCanvas()
        {
            DefaultCanvasHandle.Clear();
        }
    }
}