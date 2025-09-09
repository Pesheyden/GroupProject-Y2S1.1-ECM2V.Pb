using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/Vector3EventSo", fileName = "BV3_On")]
public class Vector3EventSo : EventSo
{
    public Vector3 Value;

    public override void Raise()
    {
        List<Vector3EventListener> vector3EventListeners = new();
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            vector3EventListeners.Add(_eventListeners[i] as Vector3EventListener);

        foreach (var vector3EventListener in vector3EventListeners)
        {
            vector3EventListener.OnEventRaised(Value);
        }
    }
}