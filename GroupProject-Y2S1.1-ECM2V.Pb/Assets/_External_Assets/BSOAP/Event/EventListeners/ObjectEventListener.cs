using UnityEngine;

public class ObjectEventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is Object value)
        {
            Response.Invoke(value);
            Debug.LogWarning(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
