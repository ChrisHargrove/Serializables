
namespace BatteryAcid.Serializables.Editor
{
    /// <summary>
    /// Class to provide the storage of a conflicting entry into the SerializedDictionary
    /// </summary>
    public class SerializableDictionaryConflict
    {
        public object Key { get; set; }
        public object Value { get; set; }
        public int Index { get; set; } = -1;
        public int OtherIndex { get; set; } = -1;
        public bool IsKeyExpanded { get; set; }
        public bool IsValueExpanded { get; set; }
        public float LineHeight { get; set; }

        public void Clear()
        {
            Key = null;
            Value = null;
            Index = -1;
            OtherIndex = -1;
            IsKeyExpanded = false;
            IsValueExpanded = false;
            LineHeight = 0f;
        }
    }
}
