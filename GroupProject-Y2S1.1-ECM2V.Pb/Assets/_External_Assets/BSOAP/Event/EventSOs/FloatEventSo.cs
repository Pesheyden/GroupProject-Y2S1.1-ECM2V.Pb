using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/FloatEventSo", fileName = "BF_On")]
public class FloatEventSo : EventSo
{
    public float Value;

    public override void Raise()
    {
        List<FloatEventListener> floatEventListeners = new();
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            floatEventListeners.Add(_eventListeners[i] as FloatEventListener);

        foreach (var floatEventListener in floatEventListeners)
        {
            floatEventListener.OnEventRaised(Value);
        }
    }
}

