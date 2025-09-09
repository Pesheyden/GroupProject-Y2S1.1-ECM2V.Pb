using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "BSOAP/Events/EventSo", fileName = "B_On")]
public class EventSo : ScriptableObject
{
    protected List<EventListener> _eventListeners = new List<EventListener>();

    public List<EventListener> EventListeners => _eventListeners;

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public virtual void Raise( )
    {
        for(int i = _eventListeners.Count - 1; i >=0; i--)
            _eventListeners[i].OnEventRaised(null);
    }

    public virtual void RegisterResponse(EventListener eventListener)
    {
        _eventListeners.Add(eventListener);
    }

    public virtual void UnRegisterResponse(EventListener eventListener)
    {
        _eventListeners.Remove(eventListener);
    }
}
