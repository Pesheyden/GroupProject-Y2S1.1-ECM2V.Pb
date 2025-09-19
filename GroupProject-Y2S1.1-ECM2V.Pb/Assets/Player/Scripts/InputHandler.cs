using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private PlayerInput _playerInput;

    public delegate void InputEvent();
    public delegate void InputEvent<T>(T parameter);

    public event InputEvent OnAnyStarted;
    public event InputEvent OnAnyPerformed;
    public event InputEvent OnAnyCanceled;

    private InputAction _move;
    public event InputEvent<Vector2> OnMoveStarted;
    public event InputEvent<Vector2> OnMovePerformed;
    public event InputEvent<Vector2> OnMoveCanceled;

    private InputAction _jump;
    public event InputEvent OnJumpStarted;
    public event InputEvent OnJumpPerformed;
    public event InputEvent OnJumpCanceled;

    private InputAction _interact;
    public event InputEvent OnInteractStarted;
    public event InputEvent OnInteractPerformed;
    public event InputEvent OnInteractCanceled;

    private InputAction _cameraMove;
    public event InputEvent<Vector2> OnCameraMoveStarted;
    public event InputEvent<Vector2> OnCameraMovePerformed;
    public event InputEvent<Vector2> OnCameraMoveCanceled;

    private InputAction _stretch;
    public event InputEvent<int> OnStretchStarted;
    public event InputEvent<int> OnStretchPerformed;
    public event InputEvent<int> OnStretchCanceled;

    private InputAction _stretch1;
    public event InputEvent<int> OnStretch1Started;
    public event InputEvent<int> OnStretch1Performed;
    public event InputEvent<int> OnStretch1Canceled;

    private void Reset()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        if (_playerInput) _playerInput = GetComponent<PlayerInput>();

        SetInstance();
        Initialize();
    }

    private void SetInstance()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    private void Initialize()
    {
        if (!_playerInput) _playerInput = GetComponent<PlayerInput>();
        if (_playerInput)
        {
            InputActionMap gameplay = _playerInput.actions.FindActionMap("Gameplay");
            if (gameplay == null)
            {
                Debug.LogError($"{this} could not find action map \"Gameplay\".");
                return;
            }
            gameplay.Enable();

            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            _playerInput.onActionTriggered += ReadContext;

            SetInputActions();
        }
    }

    private void OnEnable()
    {
        if (_playerInput) _playerInput.onActionTriggered += ReadContext;
    }

    private void OnDisable()
    {
        if (_playerInput) _playerInput.onActionTriggered -= ReadContext;
    }

    private void ReadContext(InputAction.CallbackContext context)
    {
        InputAction action = context.action;

        if (action == _move ||
            action == _jump)
        {
            if (context.started) OnAnyStarted?.Invoke();
            if (context.performed) OnAnyPerformed?.Invoke();
            if (context.canceled) OnAnyCanceled?.Invoke();
        }
        
        if (action == _move)
        {
            if (context.started) OnMoveStarted?.Invoke(context.ReadValue<Vector2>());
            if (context.performed) OnMovePerformed?.Invoke(context.ReadValue<Vector2>());
            if (context.canceled) OnMoveCanceled?.Invoke(context.ReadValue<Vector2>());
        }

        if (action == _jump)
        {
            if (context.started) OnJumpStarted?.Invoke();
            if (context.performed) OnJumpPerformed?.Invoke();
            if (context.canceled) OnJumpCanceled?.Invoke();
        }

        if (action == _interact)
        {
            if (context.started) OnInteractStarted?.Invoke();
            if (context.performed) OnInteractPerformed?.Invoke();
            if (context.canceled) OnInteractCanceled?.Invoke();
        }

        if (action == _cameraMove)
        {
            if (context.started) OnCameraMoveStarted?.Invoke(context.ReadValue<Vector2>());
            if (context.performed) OnCameraMovePerformed?.Invoke(context.ReadValue<Vector2>());
            if (context.canceled) OnCameraMoveCanceled?.Invoke(context.ReadValue<Vector2>());
        }

        if (action == _stretch)
        {
            if (context.started) OnStretchStarted?.Invoke(0);
            if (context.performed) OnStretchPerformed?.Invoke(0);
            if (context.canceled) OnStretchCanceled?.Invoke(0);
        }

        if (action == _stretch1)
        {
            if (context.started) OnStretch1Started?.Invoke(1);
            if (context.performed) OnStretch1Performed?.Invoke(1);
            if (context.canceled) OnStretch1Canceled?.Invoke(1);
        }
    }

    private void SetInputActions()
    {
        InputActionMap gameplay = _playerInput.actions.FindActionMap("Gameplay");

        if (gameplay == null)
        {
            Debug.LogError($"{this} could not find action map \"Gameplay\".");
            return;
        }

        _move = gameplay.FindAction("Move");
        _jump = gameplay.FindAction("Jump");
        _interact = gameplay.FindAction("Interact");
        _cameraMove = gameplay.FindAction("Camera Move");
        _stretch = gameplay.FindAction("Stretch");
        _stretch1 = gameplay.FindAction("Stretch1");
    }
}
