using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableType))]
public class SerializableTypeDrawer : PropertyDrawer
{
    private static readonly Type[] CommonTypes = new Type[]
    {
        typeof(int),
        typeof(float),
        typeof(bool),
        typeof(string),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Color),
        typeof(GameObject),
        typeof(MonoBehaviour),
        typeof(object),
        // add any other commonly used types here
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var typeNameProp = property.FindPropertyRelative("_typeAssemblyQualifiedName");

        // Current type
        Type currentType = string.IsNullOrEmpty(typeNameProp.stringValue) ? null : Type.GetType(typeNameProp.stringValue);

        // Prepare type names for popup
        string[] options = CommonTypes.Select(t => t.Name).ToArray();
        int selectedIndex = currentType == null ? -1 : Array.FindIndex(CommonTypes, t => t == currentType);

        EditorGUI.BeginProperty(position, label, property);
        {
            // Draw label
            position = EditorGUI.PrefixLabel(position, label);

            // Draw popup dropdown
            int newIndex = EditorGUI.Popup(position, selectedIndex, options);

            if (newIndex != selectedIndex)
            {
                Type newType = newIndex >= 0 ? CommonTypes[newIndex] : null;
                typeNameProp.stringValue = newType?.AssemblyQualifiedName ?? string.Empty;
            }
        }
        EditorGUI.EndProperty();
    }
}