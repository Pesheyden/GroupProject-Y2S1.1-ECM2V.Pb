using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(EventSo))]
public class EventPropertyDrawer : PropertyDrawer
{
    private EventSo _eventSo;
    private float _propertyHeight = EditorGUIUtility.singleLineHeight;

    /// <summary>
    /// Renders the property drawer GUI.
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _eventSo = property.objectReferenceValue as EventSo;
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
            Rect raiseButtonPosition = new Rect(
                rightPartRect.x,
                rightPartRect.y,
                rightPartRect.width / 2 - 5,
                rightPartRect.height
            );

            // Set delete button Rect
            Rect deleteButtonRect = new Rect(
                rightPartRect.x + raiseButtonPosition.width + 5,
                rightPartRect.y,
                rightPartRect.width / 2 - 5,
                rightPartRect.height);

            // Draw VariableSo field without ability to edit
            GUI.enabled = false;
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect,
                property.objectReferenceValue, typeof(EventSo),
                property.serializedObject.targetObject);
            GUI.enabled = true;
            
            //Draw Raise button if game is playing
            if (Application.isPlaying)
            {
                // Draw Value property and register value changes
                if (GUI.Button(raiseButtonPosition, "Raise"))
                {
                    _eventSo.Raise();
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
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect,
                property.objectReferenceValue, typeof(EventSo),
                property.serializedObject.targetObject);

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

            //Draw value field if so is set
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
                if (valueProperty != null)
                {
                    switch (valueProperty.propertyType)
                    {
                        case SerializedPropertyType.Generic:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Integer:
                            valueProperty.intValue = EditorGUI.IntField(valuePosition, valueProperty.intValue);
                            break;
                        case SerializedPropertyType.Boolean:
                            valueProperty.boolValue = EditorGUI.Toggle(valuePosition, valueProperty.boolValue);
                            break;
                        case SerializedPropertyType.Float:
                            valueProperty.floatValue = EditorGUI.FloatField(valuePosition, valueProperty.floatValue);
                            break;
                        case SerializedPropertyType.String:
                            valueProperty.stringValue = EditorGUI.TextField(valuePosition, valueProperty.stringValue);
                            break;
                        case SerializedPropertyType.Color:
                            throw new NotImplementedException();
                        case SerializedPropertyType.ObjectReference:
                            valueProperty.objectReferenceValue = EditorGUI.ObjectField(valuePosition,valueProperty.objectReferenceValue, typeof(Object), valueProperty.serializedObject.targetObject);
                            break;
                        case SerializedPropertyType.LayerMask:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Enum:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Vector2:
                            valueProperty.vector2Value = EditorGUI.Vector2Field(valuePosition, GUIContent.none, valueProperty.vector2Value);
                            break;
                        case SerializedPropertyType.Vector3:
                            valueProperty.vector3Value = EditorGUI.Vector3Field(valuePosition, GUIContent.none, valueProperty.vector3Value);
                            break;
                        case SerializedPropertyType.Vector4:
                            valueProperty.vector4Value = EditorGUI.Vector4Field(valuePosition, GUIContent.none, valueProperty.vector4Value);
                            break;
                        case SerializedPropertyType.Rect:
                            throw new NotImplementedException();
                        case SerializedPropertyType.ArraySize:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Character:
                            throw new NotImplementedException();
                        case SerializedPropertyType.AnimationCurve:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Bounds:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Gradient:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Quaternion:
                            throw new NotImplementedException();
                        case SerializedPropertyType.ExposedReference:
                            throw new NotImplementedException();
                        case SerializedPropertyType.FixedBufferSize:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Vector2Int:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Vector3Int:
                            throw new NotImplementedException();
                        case SerializedPropertyType.RectInt:
                            throw new NotImplementedException();
                        case SerializedPropertyType.BoundsInt:
                            throw new NotImplementedException();
                        case SerializedPropertyType.ManagedReference:
                            throw new NotImplementedException();
                        case SerializedPropertyType.Hash128:
                            throw new NotImplementedException();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            // Draw VariableSo property and register value changes
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldPosition,
                property.objectReferenceValue, typeof(EventSo),
                property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
        }

    

    /// <summary>
    /// Creates a new BoolVariableSO as a sub-asset.
    /// </summary>
    public EventSo CreateSubVariable(Object parentObject, string name)
    {
        //Creating Instance and renaming it to the property name
        var variableSO = ScriptableObject.CreateInstance<EventSo>();
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



[CustomEditor(typeof(EventSo))]
public class EventSoDrawer : Editor
{
    private EventSo _eventSo;
    private void OnEnable()
    {
        _eventSo = target as EventSo;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(!Application.isPlaying)
            return;
        if(GUILayout.Button("Raise"))
            _eventSo.Raise();
        
    }
}
