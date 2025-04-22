using UnityEngine;

namespace UiProjector
{
    /// <summary>
    /// スクリーン範囲内に収まるように位置を修正するProjectionReviser
    /// </summary>
    public class ScreenReviser : RectReviserBase
    {
        public float TopMargin, BottomMargin, LeftMargin, RightMargin;

        /// <summary> コンストラクタ </summary>
        public ScreenReviser() : this(0, 0, 0, 0) { }

        /// <summary> コンストラクタ </summary>
        public ScreenReviser(float topMargin, float bottomMargin, float leftMargin, float rightMargin) => (TopMargin, BottomMargin, LeftMargin, RightMargin) = (topMargin, bottomMargin, leftMargin, rightMargin);

        /// <inheritdoc />
        protected override Rect GetRect()
        {
            var x = LeftMargin;
            var y = BottomMargin;
            var width = Screen.width - LeftMargin - RightMargin;
            var height = Screen.height - TopMargin - BottomMargin;
            return new Rect(x, y, width, height);
        }
    }

}