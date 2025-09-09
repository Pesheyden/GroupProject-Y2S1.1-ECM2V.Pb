#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.Assertions;

public static class SerializedPropertyUtility
{
    private static readonly MethodInfo getFieldInfoFromProperty;

    static SerializedPropertyUtility()
    {
        var scriptAttributeUtility = typeof(Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
        Assert.IsNotNull(scriptAttributeUtility, "ScriptAttributeUtility != null");

        getFieldInfoFromProperty = scriptAttributeUtility.GetMethod(nameof(GetFieldInfoFromProperty), BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(getFieldInfoFromProperty, "getFieldInfoFromProperty != null");
    }
    
    public static FieldInfo GetFieldInfoFromProperty(this SerializedProperty property, out Type type)
    {
        type = null;
        var fieldInfo = (FieldInfo)getFieldInfoFromProperty.Invoke(null,
            new object[]
            {
                property, type
            });
        return fieldInfo;
    }
}

#endif