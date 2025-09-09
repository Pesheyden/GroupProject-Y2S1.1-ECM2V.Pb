using UnityEngine;

public class FloatEventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is float value)
        {
            Response.Invoke(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
