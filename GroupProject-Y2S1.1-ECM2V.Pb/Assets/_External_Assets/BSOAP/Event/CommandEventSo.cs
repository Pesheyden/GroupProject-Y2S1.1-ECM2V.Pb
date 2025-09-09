using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCommands;
using BSOAP.Variables;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BSOAP.Events
{
    [CreateAssetMenu(menuName = "BSOAP/Events/CommandEventSo", fileName = "BC_On")]
    public class CommandEventSo : ScriptableObject
    {
        [Tooltip("Expected dynamic parameter types for all registered dynamic/mixed commands.")]
        public List<SerializableType> ExpectedDynamicParameters = new();

        [SerializeField] private bool _debugMod;
        [SerializeField] private VariableResetEnum _reset;

        /// <summary>
        /// Computed property returning current expected dynamic parameter Types.
        /// </summary>
        public Type[] ExpectedParameterTypes
        {
            get
            {
                return ExpectedDynamicParameters
                    .Select(t => t.Type)
                    .Where(t => t != null)
                    .ToArray();
            }
        }

        private readonly List<object> _commands = new();

        [Button("Raise")]
        public void EmptyRaise()
        {
            Raise();
        }
        
        /// <summary>
        /// Raises the event and executes all registered commands with optional dynamic parameters.
        /// </summary>
        /// <param name="dynamicParameters">Optional parameters for dynamic or mixed commands.</param>
        public void Raise(params object[] dynamicParameters)
        {
            Log("Start Raise");
            foreach (var command in _commands)
            {

                switch (command)
                {
                    case DynamicParameterCommand dynamicCmd:
                        Log($"Running: {dynamicCmd.GetKey()}");
                        dynamicCmd.Execute(dynamicParameters);
                        break;

                    case MixedCommand mixedCmd:
                        Log($"Running: {mixedCmd.GetKey()}");
                        mixedCmd.Execute(dynamicParameters);
                        break;

                    case GenericMethodCommand genericCmd:
                        Log($"Running: {genericCmd.GetKey()}");
                        genericCmd.Execute();
                        break;

                    default:
                        Debug.LogWarning($"Unsupported command type: {command?.GetType().Name}");
                        break;
                }
            }
        }

        /// <summary>
        /// Registers a command.
        /// </summary>
        /// <param name="command">Command to register.</param>
        public void RegisterCommand(object command)
        {
            if (command == null)
            {
                Debug.LogWarning("Attempted to register a null command.");
                return;
            }

            if (command is IDynamicCommand || command is MixedCommand)
            {
                var paramTypes = ExtractExpectedParameterTypes(command);
                
                if (!CompareParameterTypes(ExpectedParameterTypes, paramTypes))
                {
                    Debug.LogError("Command dynamic parameter types do not match the expected signature.");
                    return;
                }
            }

            if (!_commands.Contains(command))
            {
                _commands.Add(command);
                Log($"Command: {command} was added");
            }


        }

        /// <summary>
        /// Unregisters a command.
        /// </summary>
        /// <param name="command">Command to unregister.</param>
        public void UnregisterCommand(object command)
        {
            _commands.Remove(command);
            Log($"Command: {command} was removed" );
        }

        /// <summary>
        /// Clears all registered commands.
        /// </summary>
        public void Clear()
        {
            _commands.Clear();
            Log($"Commands were cleared");
        }

        /// <summary>
        /// Compares two parameter type arrays for equality.
        /// </summary>
        private bool CompareParameterTypes(Type[] a, Type[] b)
        {
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Extracts expected dynamic parameter types from a dynamic or mixed command.
        /// Assumes these commands expose ExpectedParameterTypes property.
        /// </summary>
        private Type[] ExtractExpectedParameterTypes(object command)
        {
            var prop = command.GetType().GetProperty("ExpectedParameterTypes");
            if (prop != null && prop.PropertyType == typeof(Type[]))
            {
                return (Type[])prop.GetValue(command);
            }

            Debug.LogWarning("Command does not expose ExpectedParameterTypes. Assuming empty type array.");
            return Array.Empty<Type>();
        }

        private void Log(string message)
        {
            if(!_debugMod)
                return;
            StringBuilder sb = new StringBuilder();
            sb.Append($"Object: {name}");
            sb.AppendLine("Message: " + message);
            Debug.Log(sb.ToString());
        }
        
         /// <summary>
        /// Method called when the scriptable object is enabled.
        /// </summary>
        public void OnEnable()
        {
            if (_debugMod)
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
                    _commands.Clear();
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
            switch (_reset)
            {
                case VariableResetEnum.None:
                    // Do nothing
                    break;
                case VariableResetEnum.OnSceneLoaded:
                    // Do nothing
                    break;
                case VariableResetEnum.OnGameStarted:
                    _commands.Clear();
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}