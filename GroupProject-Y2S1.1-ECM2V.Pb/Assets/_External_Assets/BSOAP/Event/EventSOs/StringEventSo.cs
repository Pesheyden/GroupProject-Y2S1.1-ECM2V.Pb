using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/StringEventSo", fileName = "BS_On")]
public class StringEventSo : EventSo
{
    public string Value;

    public override void Raise()
    {
        List<StringEventListener> stringEventListeners = new();
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            stringEventListeners.Add(_eventListeners[i] as StringEventListener);

        foreach (var stringEventListener in stringEventListeners)
        {
            stringEventListener.OnEventRaised(Value);
        }
    }
}


