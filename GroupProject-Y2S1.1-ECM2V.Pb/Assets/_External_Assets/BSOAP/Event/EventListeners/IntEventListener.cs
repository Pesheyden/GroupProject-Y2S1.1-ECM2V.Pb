using UnityEngine;

public class IntEventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is int value)
        {
            Response.Invoke(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
