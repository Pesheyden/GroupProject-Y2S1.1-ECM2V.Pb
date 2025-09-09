using System;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a double variable.
    /// </summary>
    [Serializable]
    public class DoubleVariable
    {
        /// <summary>
        /// Delegate for handling double value changes.
        /// </summary>
        /// <param name="value">The new double value.</param>
        public delegate void DoubleDelegate(double value);

        /// <summary>
        /// Event triggered when the double value changes.
        /// </summary>
        public event DoubleDelegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the double value.
        /// </summary>
        public DoubleVariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the double value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public double Value
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