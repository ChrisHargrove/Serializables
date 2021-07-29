
namespace BatteryAcid.Serializables.Editor
{
    public class SerializableConflict
    {
        public object Value { get; set; }
        public int Index { get; set; } = -1;
        public int OtherIndex { get; set; } = -1;
        public bool IsValueExpanded { get; set; }
        public float LineHeight { get; set; }

        public bool IsConflicting => Index != -1 || OtherIndex != -1;

        public virtual void Clear()
        {
            Value = null;
            Index = -1;
            OtherIndex = -1;
            IsValueExpanded = false;
            LineHeight = 0f;
        }
    }

    /// <summary>
    /// Class to provide the storage of a conflicting entry into the SerializedDictionary
    /// </summary>
    public class SerializableDictionaryConflict : SerializableConflict
    {
        public object Key { get; set; }
        public bool IsKeyExpanded { get; set; }

        public override void Clear()
        {
            base.Clear();
            Key = null;
            IsKeyExpanded = false;
        }
    }
}
