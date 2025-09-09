using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BSOAP/Events/ObjectEventSo", fileName = "BO_On")]
public class ObjectEventSo : EventSo
{
    public Object Value;

    public override void Raise()
    {
        List<ObjectEventListener> objectEventListeners = new();
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            objectEventListeners.Add(_eventListeners[i] as ObjectEventListener);

        foreach (var objectEventListener in objectEventListeners)
        {
            objectEventListener.OnEventRaised(Value);
        }
    }
}