using System;
using UnityEngine;  

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a Vector4 variable.
    /// </summary>
    [Serializable]
    public class Vector4Variable
    {
        /// <summary>
        /// Delegate for handling Vector4 value changes.
        /// </summary>
        /// <param name="value">The new Vector4 value.</param>
        public delegate void Vector4Delegate(Vector4 value);

        /// <summary>
        /// Event triggered when the Vector4 value changes.
        /// </summary>
        public event Vector4Delegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the Vector4 value.
        /// </summary>
        public Vector4VariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the Vector4 value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public Vector4 Value
        {
            get => VariableSo.Value;
            set
            {
                VariableSo.Value = value;
                OnValueChanged?.Invoke(VariableSo.Value);
            } 
        }
    }
}