
namespace TeaSpoons.PackageCore.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;

    [CustomPropertyDrawer(typeof(PlayModeEditable<>), true)]
    public class PlayModeEditableDrawer : PropertyDrawer
    {
        private static readonly object[] methodCallParameters = new object[2];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var valueProperty = property.FindPropertyRelative(PlayModeEditable<object>.PropertyNames.Value);
            var previousValue = valueProperty.boxedValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, valueProperty, label);
            if (EditorGUI.EndChangeCheck())
            {
                InvokeOnUpdateEvent(property, previousValue, valueProperty.boxedValue);
            }
            
            EditorGUI.EndProperty();
        }

        private void InvokeOnUpdateEvent(SerializedProperty property, object previousValue, object newValue)
        {
            if (property.TryGetTargetObject(out var target))
            {
                var method = target.GetType().GetMethod(PlayModeEditable<object>.PropertyNames.InvokeOnUpdateEvent, BindingFlags.Instance | BindingFlags.NonPublic);

                methodCallParameters[0] = previousValue;
                methodCallParameters[1] = newValue;
                method.Invoke(target, methodCallParameters);
            }
        }
    }
}
