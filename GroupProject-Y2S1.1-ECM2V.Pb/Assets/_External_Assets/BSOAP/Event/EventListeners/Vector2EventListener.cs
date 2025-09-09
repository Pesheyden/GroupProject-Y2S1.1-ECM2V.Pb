using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Vector2EventListener : EventListener
{
    public override void OnEventRaised(object rawValue)
    {
        if (rawValue is Vector2 value)
        {
            Response.Invoke(value);
            Debug.LogWarning(value);
        }

        else
            Debug.LogError($"Input value is wrong in {gameObject.name}");
    }
}
