namespace UiProjector.Internal
{
    /// <summary>
    /// UIのID
    /// </summary>
    internal readonly struct UiId
    {
        public int Value { get; }

        public UiId(int id) => Value = id;

        public static implicit operator int(UiId id) => id.Value;
        public static implicit operator UiId(int id) => new UiId(id);
    }

    /// <summary>
    /// CanvasのID
    /// </summary>
    internal readonly struct CanvasId
    {
        public int Value { get; }

        public CanvasId(int id) => Value = id;

        public static implicit operator int(CanvasId id) => id.Value;
        public static implicit operator CanvasId(int id) => new CanvasId(id);
    }
}