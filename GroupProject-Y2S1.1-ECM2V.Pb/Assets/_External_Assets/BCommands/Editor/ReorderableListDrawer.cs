// Editor/ReorderableListDrawer.cs
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;

public static class ReorderableListDrawer
{
    /// <summary>
    /// Creates and returns a configured ReorderableList for a serialized array/list property.
    /// You should cache the returned ReorderableList (per-property) to preserve state.
    /// </summary>
    /// <param name="serializedObject">The serialized object owning the property (usually property.serializedObject).</param>
    /// <param name="listProperty">The SerializedProperty representing an array or list.</param>
    /// <param name="label">Label to draw as header.</param>
    /// <param name="drawElement">Callback to draw each element. Signature: (rect, elementProp, index, isActive, isFocused)</param>
    /// <param name="elementHeightCallback">Optional callback to get element height. If null, uses EditorGUI.GetPropertyHeight.</param>
    public static ReorderableList Create(SerializedObject serializedObject,
                                         SerializedProperty listProperty,
                                         GUIContent label,
                                         Action<Rect, SerializedProperty, int, bool, bool> drawElement,
                                         Func<SerializedProperty, int, float> elementHeightCallback = null)
    {
        if (listProperty == null) throw new ArgumentNullException(nameof(listProperty));
        if (!listProperty.isArray) throw new ArgumentException("listProperty must be an array or list.");

        var list = new ReorderableList(serializedObject, listProperty, true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, label);
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            // shrink rect a bit
            rect.y += 2;
            rect.height = elementHeightCallback != null ? elementHeightCallback(element, index) : EditorGUI.GetPropertyHeight(element, true);
            drawElement(rect, element, index, isActive, isFocused);
        };

        list.elementHeightCallback = (int index) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            if (elementHeightCallback != null) return elementHeightCallback(element, index);
            return EditorGUI.GetPropertyHeight(element, true) + 4; // small padding
        };

        list.onAddCallback = (ReorderableList rl) =>
        {
            list.serializedProperty.arraySize++;
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            // Optionally clear the newly added element:
            var newElem = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
            newElem.ClearArray(); // safe for arrays; might be no-op for non-array types
        };

        list.onRemoveCallback = (ReorderableList rl) =>
        {
            if (EditorUtility.DisplayDialog("Remove element?", $"Remove element {rl.index}?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(rl);
                rl.serializedProperty.serializedObject.ApplyModifiedProperties();
            }
        };

        return list;
    }

    /// <summary>
    /// Draws the given ReorderableList and handles serializedObject update/apply.
    /// Call inside OnGUI of a PropertyDrawer or Editor.
    /// </summary>
    public static void Draw(ReorderableList list)
    {
        if (list == null) return;
        // Ensure property is up-to-date
        list.serializedProperty.serializedObject.Update();
        list.DoLayoutList();
        list.serializedProperty.serializedObject.ApplyModifiedProperties();
    }
}
