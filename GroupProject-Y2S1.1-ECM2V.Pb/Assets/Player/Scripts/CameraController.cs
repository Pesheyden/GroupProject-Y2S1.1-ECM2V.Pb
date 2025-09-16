using UnityEngine;

[DefaultExecutionOrder(0)]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    public Transform Target => _target;

    [Header("Parts")]
    [SerializeField] private Transform _horizontalJoint;
    [SerializeField] private Transform _verticalJoint;
    [SerializeField] private Camera _camera;
    public Camera Camera => _camera;

    public enum Perspective { FirstPerson, ThirdPerson }
    [Header("Variables")]
    [SerializeField] private Perspective _perspective = Perspective.FirstPerson;

    [Space]
    [SerializeField] private Vector3 _anchorOffset = new Vector3(0.0f, 1.5f, 0.5f);
    [SerializeField] private float _minVerticalAngle = -60f;
    [SerializeField] private float _maxVerticalAngle = 60f;

    private Vector2 _moveInput;
    [Space][SerializeField] private Vector2 _moveSensitivity = Vector2.one;

    [Space]
    [SerializeField] private Vector3 _firstPersonCameraOffset;
    [SerializeField] private Vector3 _thirdPersonCameraOffset = new Vector3(0f, 1f, -5f);
    [SerializeField] private LayerMask _clipMask = ~0;

    private void OnEnable()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        SubscribeToInputActions();
    }

    private void OnDisable()
    {
        //Cursor.lockState = CursorLockMode.None;
        UnsubscribeFromInputActions();
    }

    private void SubscribeToInputActions()
    {
        if (!InputHandler.Instance) return;

        InputHandler.Instance.OnCameraMovePerformed += SetMoveInput;
        InputHandler.Instance.OnCameraMoveCanceled += SetMoveInput;
    }

    private void UnsubscribeFromInputActions()
    {
        if (!InputHandler.Instance) return;

        InputHandler.Instance.OnCameraMovePerformed -= SetMoveInput;
        InputHandler.Instance.OnCameraMoveCanceled -= SetMoveInput;
    }

    private void LateUpdate()
    {
        MoveAnchor();

        RotateHorizontalJoint();
        RotateVerticalJoint();

        MoveCamera(_perspective);
        RotateCamera(_perspective);
    }

    private void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }

    private void MoveAnchor()
    {
        if (!_target) return;

        Vector3 offset = _anchorOffset.x * _target.right + _anchorOffset.y * _target.up + _anchorOffset.z * _target.forward;
        transform.position = _target.position + offset;
    }

    private void RotateHorizontalJoint()
    {
        float angle = _horizontalJoint.localEulerAngles.y;
        angle += Time.deltaTime * _moveSensitivity.x * _moveInput.x;

        Quaternion rotation = new Quaternion { eulerAngles = new Vector3(0f, angle, 0f) };
        _horizontalJoint.localRotation = rotation;
    }

    private void RotateVerticalJoint()
    {
        float angle = _verticalJoint.localEulerAngles.x;

        angle += Time.deltaTime * _moveSensitivity.y * -_moveInput.y;
        angle = angle > 180f ? angle - 360f : angle;
        angle = Mathf.Clamp(angle, _minVerticalAngle, _maxVerticalAngle);

        Quaternion rotation = new Quaternion { eulerAngles = new Vector3(angle, 0f, 0f) };
        _verticalJoint.localRotation = rotation;
    }

    private void MoveCamera(Perspective perspective)
    {
        switch (perspective)
        {
            case Perspective.FirstPerson:
                FirstPerson();
                break;
            case Perspective.ThirdPerson:
                ThirdPerson();
                break;
        }

        void FirstPerson()
        {
            _camera.transform.localPosition = _firstPersonCameraOffset;
        }

        void ThirdPerson()
        {
            RaycastHit hit = CameraHit();

            Vector3 position = hit.collider ? hit.point + hit.normal * 0.5f : _verticalJoint.TransformPoint(_thirdPersonCameraOffset);
            _camera.transform.position = position;
        }

        RaycastHit CameraHit()
        {
            Vector3 origin = _verticalJoint.position;
            Vector3 direction = _verticalJoint.TransformDirection(_thirdPersonCameraOffset).normalized;

            RaycastHit hit;
            Physics.Raycast(origin, direction, out hit, _thirdPersonCameraOffset.magnitude, _clipMask, QueryTriggerInteraction.Ignore);

            return hit;
        }
    }

    private void RotateCamera(Perspective perspective)
    {
        switch (perspective)
        {
            case Perspective.FirstPerson:
                FirstPerson();
                break;
            case Perspective.ThirdPerson:
                ThirdPerson();
                break;
        }

        void FirstPerson()
        {
            _camera.transform.localRotation = Quaternion.identity;
        }

        void ThirdPerson()
        {
            Vector3 forward = (transform.position - _camera.transform.position).normalized;
            Vector3 upwards = Vector3.up;

            Vector3.OrthoNormalize(ref forward, ref upwards);

            Quaternion lookRotation = Quaternion.LookRotation(forward, upwards);
            _camera.transform.rotation = lookRotation;
        }
    }

    public void SetTarget(Transform target) => _target = target;

    public void SetPerspective(Perspective perspective)
    {
        _perspective = perspective;
    }
}
