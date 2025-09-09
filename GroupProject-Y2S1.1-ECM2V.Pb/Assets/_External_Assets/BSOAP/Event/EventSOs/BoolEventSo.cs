using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/BoolEventSo", fileName = "BB_On")]
public class BoolEventSo : EventSo
{
    public bool Value;
    
    public override void Raise()
    {
        List<BoolEventListener> boolEventListeners = new(); 
        for(int i = _eventListeners.Count - 1; i >=0; i--)
            boolEventListeners.Add(_eventListeners[i] as BoolEventListener);

        foreach (var boolEventListener in boolEventListeners)
        {
            boolEventListener.OnEventRaised(Value);
        }
    }
}


