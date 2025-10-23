using System;
using Joa.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class JoaInputReader : MonoBehaviour, IInputReader
{
    [Header("Input Actions")]
    [Tooltip("Vector2 Movement")]
    [SerializeField] private InputActionReference _moveAction;
    [Tooltip("Vector2 Camera Rotation")]
    [SerializeField] private InputActionReference _lookAction;
    [Tooltip("Button Jump")]
    [SerializeField] private InputActionReference _jumpAction;
    
    /// <summary>
    /// Mouse look sensitivity expressed as degrees per pixel (deg/px) of cursor movement.
    /// </summary>
    /// <remarks>
    /// This multiplier is applied to Mouse.delta each frame to
    /// accumulate yaw/pitch. Higher values rotate the camera faster. Typical
    /// desktop ranges are [0.05, 0.30].
    /// <para>
    /// This setting only affects mouse input; gamepad right–stick look is governed by
    /// <see cref="_gamepadSensitivity"/> (degrees per second).
    /// </para>
    /// </remarks>
    /// <value>Degrees rotated per pixel of mouse movement each frame.</value>
    [Header("Settings")]
    [Tooltip("Degrees per pixel traveled by cursor each frame")]
    [SerializeField] private float _mouseLookSensitivity = 0.1f;
    [Tooltip("Degrees per second when gamepad stick is fully tilted")]
    [SerializeField] private float _gamepadLookSpeed = 180f;

    public event Action<Vector2> Move;
    public event Action<Vector2> Look;
    public event Action Jump;
    
    public string CurrentScheme => _playerInput?.currentControlScheme;
    public PlayerInput PlayerInput => _playerInput;
    
    private PlayerInput _playerInput;
    private InputAction _move, _look, _jump, _interact, _pause;
    private Vector2 _stickLookRate;
    private bool _attached, _disposed;

    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        Attach(playerInput);
    }
    
    private void Update()
    {
        if (_stickLookRate != Vector2.zero)
        {
            Look?.Invoke(_stickLookRate * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Attaches this <see cref="IInputReader"/> to a specific <see cref="PlayerInput"/> instance and
    /// wires up the input action callbacks used by this component.
    /// </summary>
    /// <param name="playerInput">
    /// The <see cref="PlayerInput"/> that owns the per-instance
    /// <see cref="InputActionAsset"/> clone to read actions from.
    /// </param>
    /// <remarks>
    /// <para>
    /// If this <see cref="IInputReader"/> is already attached to another instance, it first calls
    /// <see cref="Detach"/> to unhook previous subscriptions and clear cached state.
    /// </para>
    /// <para>
    /// Actions are resolved by GUID from the serialized
    /// <see cref="InputActionReference"/> fields (_moveAction, _lookAction,
    /// _jumpAction) against <see cref="PlayerInput.actions"/>.
    /// </para>
    /// <para>
    /// The method subscribes to <see cref="InputAction.performed"/> and
    /// <see cref="InputAction.canceled"/> for each action, calls <see cref="Enable"/>
    /// to activate them, and marks the <see cref="IInputReader"/> as attached. Calling this method multiple
    /// times is safe; prior hooks are removed by <see cref="Detach"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// Typical usage from a join callback:
    /// <code>
    /// void OnPlayerJoined(PlayerInput playerInput)
    /// {
    ///     IInputReader reader = playerInput.GetComponent&lt;MonoBehaviour&gt;().OfType&lt;IInputReader&gt;().FirstOrDefault()!;
    ///     reader.Attach(playerInput);
    /// }
    /// </code>
    /// </example>
    public void Attach(PlayerInput playerInput)
    {
        if (_attached) Detach();
        
        _playerInput = playerInput;

        InputActionAsset actions = _playerInput.actions;
        _move = actions.FindAction(_moveAction.action.id);
        _look = actions.FindAction(_lookAction.action.id);
        _jump = actions.FindAction(_jumpAction.action.id);

        _move.performed += OnMovePerformed;
        _move.canceled  += OnMoveCanceled;
        _look.performed += OnLookPerformed;
        _look.canceled  += OnLookCanceled;
        _jump.performed += OnJumpPerformed;
        
        Enable();
        _attached = true;
    }

    public void Detach()
    {
        if (!_attached) return;

        Disable();

        _move.performed -= OnMovePerformed;
        _move.canceled  -= OnMoveCanceled;
        _look.performed -= OnLookPerformed;
        _look.canceled  -= OnLookCanceled;
        _jump.performed -= OnJumpPerformed;

        _stickLookRate = Vector2.zero;
        _move = null;
        _look = null;
        _jump = null;
        _interact = null;
        _pause = null;
        _playerInput = null;
        _attached = false;
    }

    public void Enable()
    {
        _move?.Enable();
        _look?.Enable();
        _jump?.Enable();
        _interact?.Enable();
        _pause?.Enable();
    }

    public void Disable()
    {
        _move?.Disable();
        _look?.Disable();
        _jump?.Disable();
        _interact?.Disable();
        _pause?.Disable();
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        Detach();
        _disposed = true;
    }
    
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        Move?.Invoke(value.normalized);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context) => Move?.Invoke(Vector2.zero);

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        switch (context.control?.device)
        {
            case Mouse:
                Look?.Invoke(new Vector2(value.x * _mouseLookSensitivity, -value.y * _mouseLookSensitivity));
                break;
            case Gamepad:
                _stickLookRate = new Vector2(value.x * _gamepadLookSpeed, -value.y * _gamepadLookSpeed);
                break;
            default:
                Debug.Log($"Unknown controller device: {context.control?.device}");
                break;
        }
    }

    private void OnLookCanceled(InputAction.CallbackContext context) => _stickLookRate = Vector2.zero;

    private void OnJumpPerformed(InputAction.CallbackContext context) => Jump?.Invoke();
}
