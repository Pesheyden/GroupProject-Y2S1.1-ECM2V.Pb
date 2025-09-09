using System;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a string variable.
    /// </summary>
    [Serializable]
    public class StringVariable
    {
        /// <summary>
        /// Delegate for handling string value changes.
        /// </summary>
        /// <param name="value">The new string value.</param>
        public delegate void StringDelegate(string value);

        /// <summary>
        /// Event triggered when the string value changes.
        /// </summary>
        public event StringDelegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the string value.
        /// </summary>
        public StringVariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the string value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public string Value
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