using UnityEngine;

namespace UiProjector.Internal
{
    /// <summary>
    /// RenderMode.ScreenSpaceOverlay用の投影処理（ストラテジーパターン）
    /// </summary>
    internal class OverlayProjectionStrategy : IProjectionStrategy
    {
        private readonly Camera _camera;

        /// <summary> コンストラクタ </summary>
        public OverlayProjectionStrategy(Camera camera) => _camera = camera;

        /// <inheritdoc />
        public void Project(Binding binding)
        {
            // スクリーンに投影した位置を導く
            var screenPosition = _camera.WorldToScreenPoint(binding.Target.position + binding.WorldSpaceOffset);
            screenPosition = new Vector3(screenPosition.x + binding.ScreenSpaceOffset.x, screenPosition.y + binding.ScreenSpaceOffset.y, screenPosition.z);

            // 修正する
            binding.Reviser.Revise(ref screenPosition);

            // ローカル座標へ変換
            var canvasRectTransform = binding.Ui.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, null, out var localPosition);

            // 適用
            binding.Ui.anchoredPosition = localPosition;
        }
    }
}
