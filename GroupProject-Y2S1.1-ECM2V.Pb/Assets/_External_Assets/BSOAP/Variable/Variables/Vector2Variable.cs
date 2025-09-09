using System;
using UnityEngine; 

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a Vector2 variable.
    /// </summary>
    [Serializable]
    public class Vector2Variable
    {
        /// <summary>
        /// Delegate for handling Vector2 value changes.
        /// </summary>
        /// <param name="value">The new Vector2 value.</param>
        public delegate void Vector2Delegate(Vector2 value);

        /// <summary>
        /// Event triggered when the Vector2 value changes.
        /// </summary>
        public event Vector2Delegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the Vector2 value.
        /// </summary>
        public Vector2VariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the Vector2 value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public Vector2 Value
        {
            get => VariableSo.Value;
            set
            {
                VariableSo.Value = value;
                OnValueChanged?.Invoke(VariableSo.Value);
            } 
        }

        public void ClearInvocationList()
        {
            if (OnValueChanged == null)
                return;
            
            foreach (Delegate subscriber in OnValueChanged.GetInvocationList())
            {
                OnValueChanged -= (Vector2Delegate) subscriber;
            }
        }
    }
}