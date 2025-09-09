using UnityEngine;

public class BoolEventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is bool value)
        {
            Response.Invoke(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
