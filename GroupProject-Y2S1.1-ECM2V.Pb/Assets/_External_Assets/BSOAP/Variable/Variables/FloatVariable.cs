using System;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a float variable.
    /// </summary>
    [Serializable]
    public class FloatVariable
    {
        /// <summary>
        /// Delegate for handling float value changes.
        /// </summary>
        /// <param name="value">The new float value.</param>
        public delegate void FloatDelegate(float value);

        /// <summary>
        /// Event triggered when the float value changes.
        /// </summary>
        public event FloatDelegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the float value.
        /// </summary>
        public FloatVariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the float value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public float Value
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
                OnValueChanged -= (FloatDelegate) subscriber;
            }
        }
    }
}