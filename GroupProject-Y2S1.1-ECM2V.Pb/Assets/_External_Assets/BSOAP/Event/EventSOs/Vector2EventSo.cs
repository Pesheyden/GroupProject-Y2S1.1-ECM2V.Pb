using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/Vector2EventSo", fileName = "BV2_On")]
public class Vector2EventSo : EventSo
{
    public Vector2 Value;

    public override void Raise()
    {
        List<Vector2EventListener> vector2EventListeners = new();
        for(int i = _eventListeners.Count - 1; i >= 0; i--)
            vector2EventListeners.Add(_eventListeners[i] as Vector2EventListener);

        foreach (var vector2EventListener in vector2EventListeners)
        {
            vector2EventListener.OnEventRaised(Value);
        }
    }
}