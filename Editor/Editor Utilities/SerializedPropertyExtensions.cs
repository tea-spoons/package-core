
namespace TeaSpoons.PackageCore.Editor
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Reflection;
    using UnityEditor;

    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Attempts to find the object value of the <see cref="SerializedProperty"/> as a regular managed object.
        /// </summary>
        /// <example>
        /// public Vector3 position;
        /// // ...
        /// var property = serializedObject.FindProperty("position");
        /// if (property.TryGetTargetObject(out var target))
        /// {
        ///     // target is the Vector3 instance
        /// }
        /// </example>
        public static bool TryGetTargetObject(this SerializedProperty property, out object targetObject)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            targetObject = property.serializedObject.targetObject;

            if (targetObject == null)
            {
                return false;
            }

            var path = property.propertyPath.Split('.');
            for (var i = 0; i < path.Length; i++)
            {
                var type = targetObject.GetType();

                if (IsArray(path, i))
                {
                    i++;
                    var arrayIndex = ExtractArrayIndex(path[i]);
                    targetObject = GetArrayItemAtIndex(targetObject, arrayIndex);
                }
                else
                {
                    var fieldInfo = type.GetFieldIncludingInherited(path[i]);
                    targetObject = fieldInfo.GetValue(targetObject);
                }

                if (targetObject == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to find the object value of the <see cref="SerializedProperty"/> as a regular managed object of type <typeparamref name="T"/>.
        /// </summary>
        /// <example>
        /// public Vector3 position;
        /// // ...
        /// var property = serializedObject.FindProperty("position");
        /// if (property.TryGetTargetObject<T>(out var target))
        /// {
        ///     // target is the Vector3 instance
        /// }
        /// </example>
        public static bool TryGetTargetObject<T>(this SerializedProperty property, out T targetObject)
        {
            if (property.TryGetTargetObject(out var uncastTargetObject) && uncastTargetObject is T castTargetObject)
            {
                targetObject = castTargetObject;
                return true;
            }

            targetObject = default;
            return false;
        }

        /// <summary>
        /// Attempts to return the <see cref="FieldInfo"/> of the <see cref="SerializedProperty"/>.
        /// </summary>
        /// <example>
        /// public Vector3 position;
        /// // ...
        /// var property = serializedObject.FindProperty("position");
        /// if (property.TryGetField(out var field))
        /// {
        ///     // target is the Vector3 field
        /// }
        /// </example>
        public static bool TryGetField(this SerializedProperty property, out FieldInfo fieldInfo)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            object targetObject = property.serializedObject.targetObject;
            fieldInfo = null;

            var path = property.propertyPath.Split('.');
            for (var i = 0; i < path.Length; i++)
            {
                if (targetObject == null)
                {
                    return false;
                }

                var type = targetObject.GetType();

                if (IsArray(path, i))
                {
                    i++;
                    var arrayIndex = ExtractArrayIndex(path[i]);
                    targetObject = GetArrayItemAtIndex(targetObject, arrayIndex);

                    fieldInfo = null;
                }
                else
                {
                    fieldInfo = type.GetFieldIncludingInherited(path[i]);
                    targetObject = fieldInfo.GetValue(targetObject);

                    if (fieldInfo == null)
                    {
                        return false;
                    }
                }
            }

            return fieldInfo != null;
        }

        /// <summary>
        /// Attempts to return the attribute of type <typeparamref name="T"/>, if one is added to the field represneted by the given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <example>
        /// [TextArea]
        /// public string text;
        /// // ...
        /// var property = serializedObject.FindProperty("position");
        /// if (property.TryGetAttribute&lt;TextAreaAttribute&gt;(out var attribute))
        /// {
        ///     // target field has a [TextArea] attribute
        /// }
        /// </example>
        public static bool TryGetAttribute<T>(this SerializedProperty property, out T attribute)
            where T : Attribute
        {
            if (property.TryGetField(out var fieldInfo))
            {
                attribute = fieldInfo.GetCustomAttribute<T>();
                return attribute != null;
            }

            attribute = null;
            return false;
        }

        /// <summary>
        /// Same as GetField with predefined BindingFlags (instance | public | nonpublic),
        /// but finds fields anywhere in the given <paramref name="type"/>'s type hierarchy.
        /// </summary>
        private static FieldInfo GetFieldIncludingInherited(this Type type, string name)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            FieldInfo field = null;
            while (type != null && field == null)
            {
                field = type.GetField(name, bindingFlags);
                type = type.BaseType;
            }

            return field;
        }

        /// <summary>
        /// Treats <paramref name="targetObject"/> as an array or <see cref="ICollection"/> and returns the element in it at index <paramref name="index"/>.
        /// </summary>
        private static object GetArrayItemAtIndex(object targetObject, int index)
        {
            if (targetObject is Array array)
            {
                targetObject = array.GetValue(index);
            }
            else if (targetObject is ICollection collection)
            {
                targetObject = collection.Cast<object>().ElementAtOrDefault(index);
            }
            else
            {
                targetObject = null;
            }

            return targetObject;
        }

        /// <summary>
        /// Returns <c>true</c> if the property at the given <paramref name="index"/> in the <paramref name="propertyPath"/> if an array.
        /// </summary>
        /// <remarks>
        /// The pattern for arrays in a propertyPath is <c>"Array.data[index]"</c>.
        /// </remarks>
        private static bool IsArray(string[] propertyPath, int index)
        {
            return propertyPath.Length >= index && propertyPath[index] == "Array" && propertyPath[index + 1].StartsWith("data[");
        }

        /// <summary>
        /// Extracts the index from a string with the pattern <c>"data[index]"</c>.
        /// </summary>
        private static int ExtractArrayIndex(string s)
        {
            var indexStart = "data[".Length;
            var indexLength = s.Length - indexStart - 1;
            return int.Parse(s.Substring(indexStart, indexLength));
        }
    }
}
