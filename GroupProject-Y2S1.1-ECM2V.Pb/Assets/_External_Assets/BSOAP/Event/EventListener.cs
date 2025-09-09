using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [SerializeField] private EventSo _event;
    public UnityEvent<object> Response;
    public virtual void OnEventRaised(object value)
    {
        Response.Invoke(value);
    }

    private void OnEnable()
    {
        _event?.RegisterResponse(this);
    }

    private void OnDisable()
    {
        _event?.UnRegisterResponse(this);
    }
}
