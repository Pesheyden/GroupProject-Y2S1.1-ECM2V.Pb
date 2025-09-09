using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace BSOAP.Variables.Drawers
{
    /// <summary>
    /// Custom property drawer for FloatVariable.
    /// </summary>
    [CustomPropertyDrawer(typeof(FloatVariable))]
    public class FloatVariableDrawer : PropertyDrawer
    {
        private bool _isPropertyShown = false;
        private float _propertyHeight = EditorGUIUtility.singleLineHeight;
        
        /// <summary>
        /// Renders the property drawer GUI.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get field info and type from the property
            var fieldInfo = property.GetFieldInfoFromProperty(out Type type);
            if (property.serializedObject.targetObject.GetType().BaseType == typeof(ScriptableObject))
            {
                // Check for SubAsset attributes
                var attributes = (SubAsset[])fieldInfo.GetCustomAttributes<SubAsset>();
                if (attributes.Length > 0)
                {
                    DrawSubAssetProperty(position, property, label);
                    return;
                }
            }

            DrawStandardProperty(position, property, label);
        }

        /// <summary>
        /// Gets the height of the property drawer.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _propertyHeight;
        }

        /// <summary>
        /// Draws the GUI for a SubAsset property.
        /// </summary>
        private void DrawSubAssetProperty(Rect position, SerializedProperty property, GUIContent label)
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
            
            // Set basic object section position
            // Object section is a part right to the label 
            Rect objectSectionRect = new Rect(
                position.position.x + labelPosition.width,
                position.y,
                position.width - labelPosition.width,
                EditorGUIUtility.singleLineHeight
                );
            
            // Get SerializedProperty of VariableSo
            SerializedProperty variableProperty = property.FindPropertyRelative("VariableSo");
            
            //Draw object section and additional properties
            if (variableProperty.objectReferenceValue != null)
            {
                // If Scriptable object is set
                
                // Set object field Rect
                Rect objectFieldRect = objectSectionRect;
                objectFieldRect.width /= 2;
                objectFieldRect.width -= 10;
                
                //Set Rect od the part on the right of the object field
                Rect rightPartRect = objectFieldRect;
                rightPartRect.position = new Vector2(
                    objectFieldRect.position.x + objectFieldRect.width + 10, 
                    objectFieldRect.position.y
                    );
                
                // Set show or hide button Rect
                Rect showHideButtonRect = new Rect(
                    rightPartRect.x, 
                    rightPartRect.y, 
                    rightPartRect.width / 2 - 5, 
                    rightPartRect.height
                    );
                
                // Set delete button Rect
                Rect deleteButtonRect = new Rect(
                    rightPartRect.x + showHideButtonRect.width + 5,
                    rightPartRect.y, 
                    rightPartRect.width / 2 - 5, 
                    rightPartRect.height);

                // Draw VariableSo field without ability to edit
                GUI.enabled = false;
                variableProperty.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, variableProperty.objectReferenceValue, typeof(FloatVariableSO), variableProperty.serializedObject.targetObject);
                GUI.enabled = true;

                // Drawing hide or show button and additional properties 
                if (_isPropertyShown)
                {
                    // Draw hide button and add logic
                    if (GUI.Button(showHideButtonRect, "Hide"))
                    {
                        _isPropertyShown = false;
                        _propertyHeight = EditorGUIUtility.singleLineHeight;
                    }

                    // Draw the value field of the variable
                    // Set value field Rect
                    Rect valuePosition = new Rect(
                        objectSectionRect.x,
                        objectSectionRect.y + EditorGUIUtility.singleLineHeight + 2,
                        objectSectionRect.width,
                        EditorGUIUtility.singleLineHeight
                        );
                    // Get Value property
                    var serializedObject = new SerializedObject(variableProperty.objectReferenceValue);
                    var valueProperty = serializedObject.FindProperty("_value");
                    
                    // Draw Value property and register value changes;
                    valueProperty.floatValue = EditorGUI.FloatField(valuePosition, valueProperty.floatValue);
                    valueProperty.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    // Draw Show button and add logic
                    if (GUI.Button(showHideButtonRect, "Show"))
                    {
                        _isPropertyShown = true;
                        _propertyHeight = EditorGUIUtility.singleLineHeight * 2;
                    }
                }

                // Draw delete Button
                if (GUI.Button(deleteButtonRect, "X"))
                {
                    DeleteSubVariable(variableProperty.objectReferenceValue);
                    variableProperty.objectReferenceValue = null;
                }
            }
            else
            {
                // If scriptable object is not set
                
                
                // Set object field Rect
                Rect objectFieldRect = objectSectionRect;
                objectFieldRect.width /= 2;
                objectFieldRect.width -= 10;
                
                // Set create button Rect
                Rect createButtonRect = new Rect(
                    objectFieldRect.x + objectFieldRect.width + 10,
                    objectFieldRect.y,
                    objectFieldRect.width,
                    objectFieldRect.height);
                
                // Draw VariableSo property and register value changes
                variableProperty.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, variableProperty.objectReferenceValue, typeof(FloatVariableSO), variableProperty.serializedObject.targetObject);
                
                // Draw create Button
                if (GUI.Button(createButtonRect, "Create"))
                {
                    // Create new instance of the VariableSO and set it
                    variableProperty.objectReferenceValue = CreateSubVariable(property.serializedObject.targetObject, property.name);
                }
            }

            variableProperty.serializedObject.ApplyModifiedProperties();
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
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);
            
            // Draw the object field for the variable
            Rect objectFieldPosition = new Rect(
                position.position.x + labelPosition.width,
                position.y, 
                position.width - labelPosition.width,
                EditorGUIUtility.singleLineHeight);
            SerializedProperty variableProperty = property.FindPropertyRelative("VariableSo");
            
            // Draw value field if ValueSo is set
            if (variableProperty.objectReferenceValue != null)
            {
                // Change object field Rect
                objectFieldPosition.width /= 2;
                objectFieldPosition.width -= 10;
                
                // Set value field Rect
                Rect valuePosition = new Rect(
                    objectFieldPosition.x + objectFieldPosition.width + 10,
                    objectFieldPosition.y,
                    objectFieldPosition.width,
                    EditorGUIUtility.singleLineHeight);
                
                // Get Value property
                var serializedObject = new SerializedObject(variableProperty.objectReferenceValue);
                var valueProperty = serializedObject.FindProperty("_value");
                
                // Draw Value property and register value changes
                valueProperty.floatValue = EditorGUI.FloatField(valuePosition, valueProperty.floatValue);
                serializedObject.ApplyModifiedProperties();
            }

            variableProperty.objectReferenceValue = EditorGUI.ObjectField(objectFieldPosition, variableProperty.objectReferenceValue, typeof(FloatVariableSO), variableProperty.serializedObject.targetObject);
            variableProperty.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
        }
        /// <summary>
        /// Creates a new BoolVariableSO as a sub-asset.
        /// </summary>
        public FloatVariableSO CreateSubVariable(Object parentObject, string name)
        {
            //Creating Instance and renaming it to the property name
            var variableSO = ScriptableObject.CreateInstance<FloatVariableSO>();
            variableSO.name = name;

            //Saving Asset to the Stats object
            AssetDatabase.AddObjectToAsset(variableSO, parentObject);
            AssetDatabase.SaveAssets();

            return variableSO;
        }

        /// <summary>
        /// Deletes a sub-asset.
        /// </summary>
        public void DeleteSubVariable(Object variableSO)
        {
            AssetDatabase.RemoveObjectFromAsset(variableSO);
            AssetDatabase.SaveAssets();
        }
    }
}
