using System;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents an integer variable.
    /// </summary>
    [Serializable]
    public class IntVariable
    {
        /// <summary>
        /// Delegate for handling integer value changes.
        /// </summary>
        /// <param name="value">The new integer value.</param>
        public delegate void IntDelegate(int value);

        /// <summary>
        /// Event triggered when the integer value changes.
        /// </summary>
        public event IntDelegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the integer value.
        /// </summary>
        public IntVariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the integer value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public int Value
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