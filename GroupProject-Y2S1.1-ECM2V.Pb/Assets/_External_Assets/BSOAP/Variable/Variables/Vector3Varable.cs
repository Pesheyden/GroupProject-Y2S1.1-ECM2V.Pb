using System;
using UnityEngine;  

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a Vector3 variable.
    /// </summary>
    [Serializable]
    public class Vector3Variable
    {
        /// <summary>
        /// Delegate for handling Vector3 value changes.
        /// </summary>
        /// <param name="value">The new Vector3 value.</param>
        public delegate void Vector3Delegate(Vector3 value);

        /// <summary>
        /// Event triggered when the Vector3 value changes.
        /// </summary>
        public event Vector3Delegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the Vector3 value.
        /// </summary>
        public Vector3VariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the Vector3 value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public Vector3 Value
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