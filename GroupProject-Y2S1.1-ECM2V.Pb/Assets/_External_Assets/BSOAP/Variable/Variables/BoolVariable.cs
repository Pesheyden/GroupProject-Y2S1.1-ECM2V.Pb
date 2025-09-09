using System;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a boolean variable.
    /// </summary>
    [Serializable]
    public class BoolVariable
    {
        /// <summary>
        /// Delegate for handling boolean value changes.
        /// </summary>
        /// <param name="value">The new boolean value.</param>
        public delegate void BoolDelegate(bool value);

        /// <summary>
        /// Event triggered when the boolean value changes.
        /// </summary>
        public BoolDelegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the boolean value.
        /// </summary>
        public BoolVariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the boolean value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public bool Value
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
                OnValueChanged -= (BoolDelegate) subscriber;
            }
        }
    }
}