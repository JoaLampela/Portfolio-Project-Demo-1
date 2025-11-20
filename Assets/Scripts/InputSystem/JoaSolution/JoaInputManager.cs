using System;
using System.Linq;
using System.Threading.Tasks;
using Joa.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[RequireComponent(typeof(PlayerInputManager))]
public sealed class JoaInputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _playerPrefab;
    
    [Header("Services")]
    [SerializeField] private Object _bindingStore; // Sample service for demonstration purposes
    private IBindingStore BindingStore => _bindingStore as IBindingStore;
    
    private PlayerInputManager _playerInputManager;
    
    private void Awake()
    {
        // TODO: Move these to something responsible for Player/UI toggling (pause menu)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.playerPrefab = _playerPrefab;
    }
    
    private void Start()
    {
        if (PlayerInputManager.instance.playerCount != 0) return;
        
        PlayerInputManager.instance.JoinPlayer();
    }
    
    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }
    
    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= OnPlayerJoined;
        _playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }
    
    // Use async void only when subscribing to events if you cannot edit them to be Func<T, Task>
    // You MUST handle errors in async void, since they can brick your whole app
    private async void OnPlayerJoined(PlayerInput playerInput)
    {
        try
        {
            GameObject go = playerInput.gameObject;
            IInputReader reader = go.GetComponents<MonoBehaviour>().OfType<IInputReader>().FirstOrDefault()!;
            IPlayerController controller = go.GetComponents<MonoBehaviour>().OfType<IPlayerController>().FirstOrDefault()!;
        
            reader.Attach(playerInput);
            reader.Enable();
        
            reader.Move += controller.Move;
            reader.Look += controller.Look;
            reader.Jump += controller.Jump;
        
            // Rig other services as you add them here (UI/HUD, Glyphs, Camera Rig, Scheme Monitor, etc.)
            // Example: Loading per-player bindings from an IBindingStore
            await (BindingStore?.LoadBindingsAsync(playerInput.playerIndex.ToString(), playerInput.actions) ?? Task.CompletedTask); // Fails on first launch, loads bindings next
            await (BindingStore?.SaveBindingsAsync(playerInput.playerIndex.ToString(), playerInput.actions) ?? Task.CompletedTask); // Initializes bindings if not present
        }
        catch (OperationCanceledException)
        {
            // This would be called when an operation is canceled using a CancellationToken
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e}");
        }
    }
    
    // As above so below. As a bonus, here's a sample on how the wrapper events that conform to async Task could look like:
    /*
        public event Func<PlayerInput, CancellationToken, Task> OnPlayerJoinedAsync;
        public event Func<PlayerInput, CancellationToken, Task> OnPlayerLeftAsync;
     */
    private async void OnPlayerLeft(PlayerInput playerInput)
    {
        try
        {
            // Handle service cleanup at the end of lifecycle (IDisposables need to be Dispos()'d, etc.)
            IInputReader reader = playerInput.GetComponents<MonoBehaviour>().OfType<IInputReader>().FirstOrDefault()!;
            reader?.Dispose();

            // Handle other services here as well, the same as above
            await (BindingStore?.SaveBindingsAsync(playerInput.playerIndex.ToString(), playerInput.actions) ?? Task.CompletedTask);
        }
        catch (OperationCanceledException) {}
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e}");
        }
    }
}
