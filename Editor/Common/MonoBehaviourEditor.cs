using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BatteryAcid.Serializables.Editor
{
    /// <summary>
    /// Required class to allow for the usage of UIToolkit property drawers inside the normal inspector.
    /// <para>
    /// Hopefully Unity will get this fixed soon as this feels broken imo. https://issuetracker.unity3d.com/issues/propertydrawer-dot-createpropertygui-will-not-get-called-when-using-a-custompropertydrawer-with-a-generic-struct 
    /// </para>
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            return container;
        }
    }
}