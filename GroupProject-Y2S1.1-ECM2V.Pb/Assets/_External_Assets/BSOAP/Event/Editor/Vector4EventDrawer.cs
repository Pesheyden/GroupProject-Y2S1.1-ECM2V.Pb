using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(Vector4EventSo))]
public class Vector4EventDrawer : PropertyDrawer
{
    private bool _isPropertyShown = false;
    private float _propertyHeight = EditorGUIUtility.singleLineHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var fieldInfo = property.GetFieldInfoFromProperty(out Type type);
        if (property.serializedObject.targetObject.GetType().BaseType == typeof(ScriptableObject))
        {
            var attributes = (SubAsset[])fieldInfo.GetCustomAttributes<SubAsset>();
            if (attributes.Length > 0)
            {
                DrawSubAssetProperty(position, property, label);
                return;
            }
        }
        DrawStandardProperty(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return _propertyHeight;
    }

    private void DrawSubAssetProperty(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect labelPosition = new Rect(
            position.position.x,
            position.y,
            position.width / 3f,
            EditorGUIUtility.singleLineHeight
        );
        EditorGUI.LabelField(labelPosition, label);

        Rect objectSectionRect = new Rect(
            position.position.x + labelPosition.width,
            position.y,
            position.width - labelPosition.width,
            EditorGUIUtility.singleLineHeight
        );

        if (property.objectReferenceValue != null)
        {
            Rect objectFieldRect = objectSectionRect;
            objectFieldRect.width /= 2;
            objectFieldRect.width -= 10;

            Rect rightPartRect = objectFieldRect;
            rightPartRect.position = new Vector2(
                objectFieldRect.position.x + objectFieldRect.width + 10,
                objectFieldRect.position.y
            );

            Rect showHideButtonRect = new Rect(
                rightPartRect.x,
                rightPartRect.y,
                rightPartRect.width / 2 - 5,
                rightPartRect.height
            );

            Rect deleteButtonRect = new Rect(
                rightPartRect.x + showHideButtonRect.width + 5,
                rightPartRect.y,
                rightPartRect.width / 2 - 5,
                rightPartRect.height);

            GUI.enabled = false;
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, property.objectReferenceValue, typeof(Vector4EventSo), property.serializedObject.targetObject);
            GUI.enabled = true;

            if (_isPropertyShown)
            {
                if (GUI.Button(showHideButtonRect, "Hide"))
                {
                    _isPropertyShown = false;
                    _propertyHeight = EditorGUIUtility.singleLineHeight;
                }

                Rect valuePosition = new Rect(
                    objectSectionRect.x,
                    objectSectionRect.y + EditorGUIUtility.singleLineHeight + 2,
                    objectSectionRect.width,
                    EditorGUIUtility.singleLineHeight
                );

                var serializedObject = new SerializedObject(property.objectReferenceValue);
                var valueProperty = serializedObject.FindProperty("Value");

                valueProperty.vector4Value = EditorGUI.Vector4Field(valuePosition, "", valueProperty.vector4Value);
                valueProperty.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (GUI.Button(showHideButtonRect, "Show"))
                {
                    _isPropertyShown = true;
                    _propertyHeight = EditorGUIUtility.singleLineHeight * 2;
                }
            }

            if (GUI.Button(deleteButtonRect, "X"))
            {
                DeleteSubVariable(property.objectReferenceValue);
                property.objectReferenceValue = null;
            }
        }
        else
        {
            Rect objectFieldRect = objectSectionRect;
            objectFieldRect.width /= 2;
            objectFieldRect.width -= 10;

            Rect createButtonRect = new Rect(
                objectFieldRect.x + objectFieldRect.width + 10,
                objectFieldRect.y,
                objectFieldRect.width,
                objectFieldRect.height);

            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, property.objectReferenceValue, typeof(Vector4EventSo), property.serializedObject.targetObject);

            if (GUI.Button(createButtonRect, "Create"))
            {
                property.objectReferenceValue = CreateSubVariable(property.serializedObject.targetObject, property.name);
            }
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }
        /// <summary>
    /// Draws the standard property GUI.
    /// </summary>
    private void DrawStandardProperty(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the label
        Rect labelPosition = new Rect(
            position.position.x,
            position.y,
            position.width / 3f,
            EditorGUIUtility.singleLineHeight
        );
        EditorGUI.LabelField(labelPosition, label);

        // Draw the object field for the variable
        Rect objectFieldPosition = new Rect(
            position.position.x + labelPosition.width,
            position.y,
            position.width - labelPosition.width,
            EditorGUIUtility.singleLineHeight
        );

        //Draw value field if ValueSo is set
        if (property.objectReferenceValue != null)
        {
            //Change object field Rect
            objectFieldPosition.width /= 2;
            objectFieldPosition.width -= 10;

            //Set value field Rect
            Rect valuePosition = new Rect(
                objectFieldPosition.x + objectFieldPosition.width + 10,
                objectFieldPosition.y,
                objectFieldPosition.width,
                EditorGUIUtility.singleLineHeight);

            //Get Value property
            var serializedObject = new SerializedObject(property.objectReferenceValue);
            var valueProperty = serializedObject.FindProperty("Value");

            // Draw Value property and register value changes
            valueProperty.vector4Value = EditorGUI.Vector4Field(valuePosition,"", valueProperty.vector4Value);
            serializedObject.ApplyModifiedProperties();
        }

        // Draw VariableSo property and register value changes
        property.objectReferenceValue = EditorGUI.ObjectField(objectFieldPosition,
            property.objectReferenceValue, typeof(BoolEventSo),
            property.serializedObject.targetObject);
        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }

    public Vector4EventSo CreateSubVariable(Object parentObject, string name)
    {
        var variableSO = ScriptableObject.CreateInstance<Vector4EventSo>();
        variableSO.name = name;

        AssetDatabase.AddObjectToAsset(variableSO, parentObject);
        AssetDatabase.SaveAssets();

        return variableSO;
    }

    public void DeleteSubVariable(Object variableSO)
    {
        AssetDatabase.RemoveObjectFromAsset(variableSO);
        AssetDatabase.SaveAssets();
    }
}
