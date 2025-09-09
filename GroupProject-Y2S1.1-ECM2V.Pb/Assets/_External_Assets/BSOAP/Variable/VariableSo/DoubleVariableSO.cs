#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BSOAP.Variables
{
    /// <summary>
    /// Serializable class that represents a double scriptable object variable.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "BSOAP/Values/DoubleValue", fileName = "BD_")]
    public class DoubleVariableSO : ScriptableObject
    {
        /// <summary>
        /// Gets or sets the double value.
        /// Logs the value change if DebugMode is enabled.
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                if (DebugMode)
                    Debug.Log($"Variable {name} change value from {Value} to {value}");

                _value = value;
            }
        }

        [SerializeField] private double _value;
        [SerializeField] private bool DebugMode;
        [SerializeField] private VariableResetEnum _reset;
        private double _basicValue;

        /// <summary>
        /// Method called when the scriptable object is enabled.
        /// </summary>
        public void OnEnable()
        {
            if (DebugMode)
                Debug.Log("OnEnable");

            if (_reset == VariableResetEnum.None)
                return;

            // Subscribe to play mode state changes and scene changes
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
#endif
            SceneManager.activeSceneChanged += ActiveSceneChanged;
        }

        /// <summary>
        /// Handles scene changes and resets the value based on the reset mode.
        /// </summary>
        /// <param name="arg0">The previous scene.</param>
        /// <param name="arg1">The new scene.</param>
        private void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            switch (_reset)
            {
                case VariableResetEnum.None:
                    // Do nothing
                    break;
                case VariableResetEnum.OnSceneLoaded:
                    // Reset value on scene loaded
                    Value = _basicValue;
                    break;
                case VariableResetEnum.OnGameStarted:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#if UNITY_EDITOR
        /// <summary>
        /// Handles play mode state changes and calls the appropriate method based on the state.
        /// </summary>
        /// <param name="obj">The play mode state change.</param>
        private void PlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    // Do nothing
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    // Do nothing
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    // Call method for entering play mode
                    EnteredPlayMode();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    // Call method for exiting play mode
                    ExitingPlayMode();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }
#endif
        /// <summary>
        /// Method called when play mode is entered.
        /// </summary>
        private void EnteredPlayMode()
        {
            // Store the current value as the basic value
            _basicValue = Value;
            switch (_reset)
            {
                case VariableResetEnum.None:
                    // Do nothing
                    break;
                case VariableResetEnum.OnSceneLoaded:
                    // Do nothing
                    break;
                case VariableResetEnum.OnGameStarted:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Method called when play mode is exited.
        /// </summary>
        private void ExitingPlayMode()
        {
            switch (_reset)
            {
                case VariableResetEnum.None:
                    // Do nothing
                    break;
                case VariableResetEnum.OnSceneLoaded:
                    // Do nothing
                    break;
                case VariableResetEnum.OnGameStarted:
                    // Reset value to the basic value
                    Value = _basicValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
