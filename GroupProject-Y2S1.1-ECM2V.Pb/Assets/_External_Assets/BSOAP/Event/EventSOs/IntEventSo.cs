using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/IntEventSo", fileName = "BI_On")]
public class IntEventSo : EventSo
{
    public int Value;

    public override void Raise()
    {
        List<IntEventListener> intEventListeners = new();
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            intEventListeners.Add(_eventListeners[i] as IntEventListener);

        foreach (var intEventListener in intEventListeners)
        {
            intEventListener.OnEventRaised(Value);
        }
    }
}