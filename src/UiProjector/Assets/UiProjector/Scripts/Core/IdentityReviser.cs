using UnityEngine;

namespace UiProjector.Internal
{
    /// <summary>
    /// 何も修正を行なわないときに使われる
    /// </summary>
    internal class IdentityReviser : IProjectionReviser
    {
        /// <summary> シングルトン </summary>
        public static IdentityReviser Instance { get; } = new();

        /// <inheritdoc/>
        public void Revise(ref Vector3 screenPosition) { }
    }
}