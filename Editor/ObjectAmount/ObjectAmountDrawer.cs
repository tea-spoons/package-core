namespace TeaSpoons.PackageCore.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ObjectAmount<>))]
    public class ObjectAmountDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // make tooltips work
            var tt = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);
            label.tooltip = tt.Length > 0 && (tt[0] as TooltipAttribute) != null ? ((TooltipAttribute)tt[0]).tooltip : "";

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0; // [mwalser] don't get why I need to do this. But if I don't do it, elements will be indented and not clickable.
            
            var amountInputRect = new Rect(position.x, position.y, 35, position.height);
            var objItemRect = new Rect(amountInputRect.xMax + 5, position.y, EditorGUIUtility.currentViewWidth - position.x - 60, position.height);

            var gameObjectProperty = property.FindPropertyRelative("Obj");

            EditorGUI.PropertyField(amountInputRect, property.FindPropertyRelative("Amount"), GUIContent.none);
            EditorGUI.PropertyField(objItemRect, gameObjectProperty, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
