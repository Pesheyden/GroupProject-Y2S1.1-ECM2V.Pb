using UnityEngine;

public class Vector4EventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is Vector4 value)
        {
            Response.Invoke(value);
            Debug.LogWarning(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
