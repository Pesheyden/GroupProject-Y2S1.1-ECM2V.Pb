using UnityEngine;

public class StringEventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is string value)
        {
            Response.Invoke(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
