using UnityEngine;

namespace UiProjector
{
    /// <summary>
    /// セーフエリア範囲内に収まるように位置を修正するProjectionReviser
    /// </summary>
    public class SafeAreaReviser : RectReviserBase
    {
        public float TopMargin, BottomMargin, LeftMargin, RightMargin;

        /// <summary> コンストラクタ </summary>
        public SafeAreaReviser() : this(0, 0, 0, 0) { }

        /// <summary> コンストラクタ </summary>
        public SafeAreaReviser(float topMargin, float bottomMargin, float leftMargin, float rightMargin) => (TopMargin, BottomMargin, LeftMargin, RightMargin) = (topMargin, bottomMargin, leftMargin, rightMargin);

        /// <inheritdoc />
        protected override Rect GetRect()
        {
            var safeArea = Screen.safeArea;
            var x = safeArea.x + LeftMargin;
            var y = safeArea.y + BottomMargin;
            var width = safeArea.width - LeftMargin - RightMargin;
            var height = safeArea.height - TopMargin - BottomMargin;
            return new Rect(x, y, width, height);
        }
    }
}