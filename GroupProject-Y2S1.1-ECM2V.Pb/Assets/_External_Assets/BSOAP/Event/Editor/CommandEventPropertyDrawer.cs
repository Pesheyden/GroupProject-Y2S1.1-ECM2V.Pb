using UnityEngine;
using System;
using System.Reflection;
using BSOAP.Events;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(CommandEventSo))]
public class CommandEventPropertyDrawer : PropertyDrawer
{
    private bool _isPropertyShown = false;
    private float _propertyHeight = EditorGUIUtility.singleLineHeight;
    // Cache the ReorderableList per property path (to support multiple fields)
    private ReorderableList _list;
    private string _cachedPropertyPath;
    
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
        //Draw object section and additional properties
        if (property.objectReferenceValue != null)
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
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, property.objectReferenceValue,
                typeof(CommandEventSo), property.serializedObject.targetObject);
            GUI.enabled = true;

            // BDrawing hide or show button and additional properties 
            if (_isPropertyShown)
            {
                //Draw hide button and add logic
                if (GUI.Button(showHideButtonRect, "Hide"))
                {
                    _isPropertyShown = false;
                    _propertyHeight = EditorGUIUtility.singleLineHeight;
                }

                if (_list == null || _cachedPropertyPath != property.propertyPath)
                {
                    _cachedPropertyPath = property.propertyPath;

                    // Create a local copy to pass to factory (the helper will wrap property.serializedObject and property)
                    _list = ReorderableListDrawer.Create(property.serializedObject, new SerializedObject(property.objectReferenceValue).FindProperty("ExpectedDynamicParameters"), label,
                        // drawElement callback
                        (rect, elementProp, index, isActive, isFocused) =>
                        {
                            // For each element, draw the property field (supports nested types)
                            EditorGUI.PropertyField(rect, elementProp, GUIContent.none, true);
                        },
                        // optional height callback (use default)
                        (elementProp, index) => EditorGUI.GetPropertyHeight(elementProp, true) + 4f
                    );
                }

                // Draw the list
                ReorderableListDrawer.Draw(_list);
            }
            else
            {
                //Draw Show button and add logic
                if (GUI.Button(showHideButtonRect, "Show"))
                {
                    _isPropertyShown = true;
                    _propertyHeight = EditorGUIUtility.singleLineHeight * 2;
                }
            }

            // Draw delete Button
            if (GUI.Button(deleteButtonRect, "X"))
            {
                DeleteSubVariable(property.objectReferenceValue);
                property.objectReferenceValue = null;
            }
        }
        else
        {
            // If  scriptable object is not set


            // Set object field Rect
            Rect objectFieldRect = objectSectionRect;
            objectFieldRect.width /= 2;
            objectFieldRect.width -= 10;

            // Set create button Rect
            Rect createButtonRect = new Rect(
                    objectFieldRect.x + objectFieldRect.width + 10,
                    objectFieldRect.y,
                    objectFieldRect.width,
                    objectFieldRect.height)
                ;

            // Draw VariableSo property and register value changes
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, property.objectReferenceValue,
                typeof(CommandEventSo), property.serializedObject.targetObject);

            //Draw create Button
            if (GUI.Button(createButtonRect, "Create"))
            {
                //Create new instance of the VariableSO and set it
                property.objectReferenceValue =
                    CreateSubVariable(property.serializedObject.targetObject, property.name);
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
        
        property.objectReferenceValue = EditorGUI.ObjectField(objectFieldPosition,
            property.objectReferenceValue, typeof(CommandEventSo),
            property.serializedObject.targetObject);
        property.serializedObject.ApplyModifiedProperties();
        
        EditorGUI.EndProperty();
    }


    /// <summary>
    /// Creates a new CommandEventSo as a sub-asset.
    /// </summary>
    public CommandEventSo CreateSubVariable(Object parentObject, string name)
    {
        //Creating Instance and renaming it to the property name
        var variableSO = ScriptableObject.CreateInstance<CommandEventSo>();
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