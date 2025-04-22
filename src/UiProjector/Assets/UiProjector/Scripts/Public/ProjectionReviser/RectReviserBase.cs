using UnityEngine;

namespace UiProjector
{
    /// <summary>
    /// 矩形範囲内に収まるように位置を修正するProjectionReviserの基底
    /// </summary>
    public abstract class RectReviserBase : IProjectionReviser
    {
        /// <summary>
        /// 各Reviserが使う矩形領域を返す
        /// </summary>
        protected abstract Rect GetRect();

        /// <inheritdoc />
        public void Revise(ref Vector3 screenPosition)
        {
            // ターゲットが後方にある場合
            var targetIsBehind = screenPosition.z < 0;
            if (targetIsBehind)
            {
                screenPosition *= -1;
            }

            // 矩形内に収まらない場合
            // または、ターゲットが後方にある場合
            var rect = GetRect();
            if (!rect.Contains(screenPosition) || targetIsBehind)
            {
                // 矩形の中心から外に向かう方向を求める
                var dir = (new Vector2(screenPosition.x, screenPosition.y) - rect.center).normalized;

                // 矩形の範囲内で、dir方向に可能な限り移動した点を計算する
                float halfWidth = rect.width * 0.5f;
                float halfHeight = rect.height * 0.5f;

                // 矩形中心を原点とみなして、最大移動量を制限する
                float scaleX = (dir.x != 0) ? Mathf.Abs(halfWidth / dir.x) : float.MaxValue;
                float scaleY = (dir.y != 0) ? Mathf.Abs(halfHeight / dir.y) : float.MaxValue;
                float scale = Mathf.Min(scaleX, scaleY);

                screenPosition = rect.center + dir * scale;
            }
        }
    }
}
