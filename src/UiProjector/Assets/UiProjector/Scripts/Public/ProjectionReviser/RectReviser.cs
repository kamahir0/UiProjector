using UnityEngine;

namespace UiProjector
{
    /// <summary>
    /// 矩形範囲内に収まるように位置を修正するProjectionReviser
    /// </summary>
    public class RectReviser : RectReviserBase
    {
        /// <summary> UIを収める範囲の矩形 </summary>
        public Rect Rect;

        /// <summary> コンストラクタ </summary>
        public RectReviser(Rect rect)
        {
            Rect = rect;
        }

        /// <inheritdoc />
        protected override Rect GetRect() => Rect;
    }
}