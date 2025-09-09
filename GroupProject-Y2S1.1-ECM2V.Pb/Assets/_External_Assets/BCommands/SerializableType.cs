using System;
using UnityEngine;

[Serializable]
public class SerializableType
{
    [SerializeField, HideInInspector]
    private string _typeAssemblyQualifiedName;

    /// <summary>
    /// Gets or sets the actual Type. Setting updates the internal serialized string.
    /// </summary>
    public Type Type
    {
        get
        {
            if (string.IsNullOrEmpty(_typeAssemblyQualifiedName))
                return null;
            return Type.GetType(_typeAssemblyQualifiedName);
        }
        set
        {
            _typeAssemblyQualifiedName = value?.AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// Friendly type name for display.
    /// </summary>
    public string TypeName => Type != null ? Type.Name : "<None>";
}