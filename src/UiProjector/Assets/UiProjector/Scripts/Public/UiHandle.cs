using UiProjector.Internal;

namespace UiProjector
{
    /// <summary>
    /// ProjectionUiのUIのハンドラ
    /// </summary>
    public readonly struct UiHandle
    {
        internal UiId Id { get; }
        internal CanvasId CanvasId { get; }

        internal UiHandle(UiId id, CanvasId canvasId)
        {
            Id = id;
            CanvasId = canvasId;
        }

        /// <summary> 現在もUIの存在が有効であるか </summary>
        public bool IsValid => ProjectionUiManager.Instance.IsValidUi(CanvasId, Id);

        /// <summary>
        /// UIを解放する
        /// </summary>
        public void Release()
        {
            ProjectionUiManager.Instance.ReleaseUi(CanvasId, Id);
        }
    }
}