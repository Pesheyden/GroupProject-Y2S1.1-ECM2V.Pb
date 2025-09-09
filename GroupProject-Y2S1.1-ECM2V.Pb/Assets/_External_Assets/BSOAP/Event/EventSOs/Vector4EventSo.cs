using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/Vector4EventSo", fileName = "BV4_On")]
public class Vector4EventSo : EventSo
{
    public Vector4 Value;

    public override void Raise()
    {
        List<Vector4EventListener> vector4EventListeners = new();
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            vector4EventListeners.Add(_eventListeners[i] as Vector4EventListener);

        foreach (var vector4EventListener in vector4EventListeners)
        {
            vector4EventListener.OnEventRaised(Value);
        }
    }
}