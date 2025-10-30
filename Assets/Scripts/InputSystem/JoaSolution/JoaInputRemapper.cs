using System;
using Joa.Contracts;
using Joa.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class JoaInputRemapper : MonoBehaviour
{
    // Sample action
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private string _newKey;
    
    [Header("Services")]
    [SerializeField] private Object _bindingStore;
    private IBindingStore BindingStore => _bindingStore as IBindingStore;
    
    private PlayerInput _playerInput;
    
    // Note that this is async void, so uncaught errors are catastrophic
    private async void Start()
    {
        try
        {
            // Don't attach this sort of thing to your player, this is just for the demo.
            _playerInput = GetComponent<PlayerInput>();
        
            InputAction jump = _playerInput?.actions.FindAction(_jumpAction.action.id);
            int index = jump.GetFirstBindingIndex();

            if (index == -1) return;
            if (jump.enabled) jump.Disable(); // Need null error handling here
            
            jump.ApplyBindingOverride(index, $"<Keyboard>/{_newKey}");
            jump.Enable();
            
            await BindingStore.SaveBindingsAsync(_playerInput?.playerIndex.ToString(), _playerInput?.actions);
        }
        catch (Exception e)
        {
            Debug.Log($"Exception: {e}");
        }
    }
}
