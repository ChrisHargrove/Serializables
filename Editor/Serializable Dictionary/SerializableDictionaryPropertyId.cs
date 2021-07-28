using UnityEditor;

namespace BatteryAcid.Serializables.Editor
{
    /// <summary>
    /// Class that helps with using a SerializedProperty as a key value in a dictionary.
    /// </summary>
    public struct SerializableDictionaryPropertyId
    {
        public SerializableDictionaryPropertyId(SerializedProperty property)
        {
            Instance = property.serializedObject.targetObject;
            Path = property.propertyPath;
        }
        private readonly UnityEngine.Object Instance;
        private readonly string Path;
    }
}