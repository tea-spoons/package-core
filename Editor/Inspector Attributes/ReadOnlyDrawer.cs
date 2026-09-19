namespace TeaSpoons.PackageCore.Editor
{
    using UnityEditor;
    using UnityEngine;
    
    /// <summary>
    /// Custom drawer for ReadOnly attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGuiState = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = previousGuiState;
        }
    }
}
