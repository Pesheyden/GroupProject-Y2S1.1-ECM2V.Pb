using System.Collections;
using System.Collections.Generic;
using BCommands;
using BSOAP.Events;
using NaughtyAttributes;
using UnityEngine;

public class CommandsTest : MonoBehaviour
{
    [SerializeField] private CommandEventSo _commandEventSo;
    [SerializeField] private string _value;
    
    [Button]
    private void Register()
    {
        _commandEventSo.Clear();
        
        //var command = CommandFactory.GenericCommand(this, nameof(DebugCommand));
        //var command = CommandFactory.GenericCommand(this, nameof(DebugParCommand), "With parameter");
        //var command = CommandFactory.DynamicCommand(this, nameof(DebugDynParCommand));
        var command = CommandFactory.MixedCommand(this, nameof(DebugMixCommand), "Par1");
        
        _commandEventSo.RegisterCommand(command);
    }

    private void DebugCommand()
    {
        Debug.Log("Generic");
    }
    
    private void DebugParCommand(string par)
    {
        Debug.Log("Generic: " + par);
    }
    private void DebugParCommand(int par)
    {
        Debug.Log("Generic: " + par);
    }

    private void DebugDynParCommand(string par)
    {
        Debug.Log("Dynamic: " + par);
    }
    
    private void DebugMixCommand(string par2, string par1)
    {
        Debug.Log("Mix: " + par1 + " " + par2);
    }

    [Button]
    public void RaiseDynamic()
    {
        _commandEventSo.Raise(_value);
    }
}
