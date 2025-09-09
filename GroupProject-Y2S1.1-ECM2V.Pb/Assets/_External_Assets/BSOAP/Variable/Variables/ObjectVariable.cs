using System;
using Object = UnityEngine.Object;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents an object variable.
    /// </summary>
    [Serializable]
    public class ObjectVariable
    {
        /// <summary>
        /// Delegate for handling object value changes.
        /// </summary>
        /// <param name="value">The new object value.</param>
        public delegate void ObjectDelegate(object value);

        /// <summary>
        /// Event triggered when the object value changes.
        /// </summary>
        public event ObjectDelegate OnValueChanged;
        
        /// <summary>
        /// Scriptable object that holds the object value.
        /// </summary>
        public ObjectVariableSO VariableSo;
   
        /// <summary>
        /// Gets or sets the object value.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public object Value
        {
            get => VariableSo.Value;
            set
            {
                VariableSo.Value = (Object)value;
                OnValueChanged?.Invoke(VariableSo.Value);
            } 
        }
    }
}