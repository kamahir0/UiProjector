using System;
using UnityEngine;
using UiProjector.Internal;

namespace UiProjector
{
    /// <summary>
    /// ProjectionUiのCanvasのハンドラ
    /// </summary>
    public readonly struct CanvasHandle : IDisposable
    {
        internal CanvasId Id { get; }

        internal CanvasHandle(CanvasId id)
        {
            Id = id;
        }

        /// <summary> 現在もCanvasの存在が有効であるか </summary>
        public bool IsValid => ProjectionUiManager.Instance.IsValidCanvas(Id);

        /// <summary>
        /// UIを追加する
        /// </summary>
        /// <param name="ui">追従させるUIのRectTransform</param>
        /// <param name="target">追従対象のTransform</param>
        /// <param name="worldSpaceOffset">ワールド空間でのオフセット</param>
        /// <param name="screenSpaceOffset">スクリーン空間でのオフセット</param>
        /// <param name="reviser">UIの位置を修正するProjectionReviser</param>
        /// <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public UiHandle AddUi(RectTransform ui, Transform target, Vector3 worldSpaceOffset, Vector2 screenSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null)
        {
            var uiId = ProjectionUiManager.Instance.AddUi(Id, ui, target, worldSpaceOffset, screenSpaceOffset, reviser, releaseAction);
            return new UiHandle(uiId, Id);
        }

        /// <summary> UIを追加する </summary> <param name="ui">追従させるUIのRectTransform</param> <param name="target">追従対象のTransform</param> <param name="worldSpaceOffset">ワールド空間でのオフセット</param> <param name="reviser">UIの位置を修正するProjectionReviser</param> <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public UiHandle AddUi(RectTransform ui, Transform target, Vector3 worldSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null) => AddUi(ui, target, worldSpaceOffset, Vector2.zero, reviser, releaseAction);

        /// <summary> UIを追加する </summary> <param name="ui">追従させるUIのRectTransform</param> <param name="target">追従対象のTransform</param> <param name="screenSpaceOffset">スクリーン空間でのオフセット</param> <param name="reviser">UIの位置を修正するProjectionReviser</param> <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public UiHandle AddUi(RectTransform ui, Transform target, Vector2 screenSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null) => AddUi(ui, target, Vector3.zero, screenSpaceOffset, reviser, releaseAction);

        /// <summary> UIを追加する </summary> <param name="ui">追従させるUIのRectTransform</param> <param name="target">追従対象のTransform</param> <param name="reviser">UIの位置を修正するProjectionReviser</param> <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
        public UiHandle AddUi(RectTransform ui, Transform target, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null) => AddUi(ui, target, Vector3.zero, Vector2.zero, reviser, releaseAction);

        /// <summary>
        /// 配下のUIを全て解放する
        /// </summary>
        public void ClearCanvas()
        {
            ProjectionUiManager.Instance.ClearCanvas(Id);
        }

        /// <summary>
        /// Canvasを破棄する
        /// </summary>
        public void Dispose()
        {
            ProjectionUiManager.Instance.DisposeCanvas(Id);
        }
    }
}