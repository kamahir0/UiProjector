using UnityEngine;

namespace UiProjector
{
    /// <summary>
    /// 投影処理で導かれたUIを位置に修正をかけるためのインターフェース
    /// </summary>
    public interface IProjectionReviser
    {
        /// <summary>
        /// 投影処理でUIの位置が導かれたときに呼び出し、UIの位置を修正する
        /// </summary>
        void Revise(ref Vector3 screenPosition);
    }
}