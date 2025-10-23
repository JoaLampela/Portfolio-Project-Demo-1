using System.Linq;
using Joa.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class JoaInputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _playerPrefab;
    
    [Header("Services")]
    [SerializeField] private MonoBehaviour _bindingStore; // Sample service for demonstration purposes
    private IBindingStore BindingStore => _bindingStore as IBindingStore;
    
    private PlayerInputManager _playerInputManager;
    
    private void Awake()
    {
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
    
    private void OnPlayerJoined(PlayerInput playerInput)
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
        BindingStore?.LoadBindings($"player{playerInput.playerIndex}", playerInput.actions);
    }
    
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        IInputReader reader = playerInput.GetComponents<MonoBehaviour>().OfType<IInputReader>().FirstOrDefault()!;
        reader?.Dispose();
    }
}
