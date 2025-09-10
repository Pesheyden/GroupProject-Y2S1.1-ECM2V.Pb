using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _target;
    void FixedUpdate()
    {
        transform.position = _target.position;
    }
}
