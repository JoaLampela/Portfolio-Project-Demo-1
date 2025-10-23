using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Joa.Contracts
{
    public interface IInputReader : IDisposable
    {
        public event Action<Vector2> Move;
        public event Action<Vector2> Look;
        public event Action Jump;
        
        public string CurrentScheme { get; }
        public PlayerInput PlayerInput { get; }
        
        public void Attach(PlayerInput playerInput);
        public void Detach();
        public void Enable();
        public void Disable();
    }
    
    public interface IPlayerController
    {
        public Transform Transform { get; } // Root GameObject of Player
        
        public void Move(Vector2 axes);
        public void Look(Vector2 deltaDegrees);
        public void Jump();
    }
    
    public interface ICameraRig
    {
        public Camera Camera { get; }
        public void AssignTarget(Transform target);
        public void SetViewport(Rect normalRect); // Split-screen support
        public void SetActive(bool active);
        public void Tick(float deltaTime); // Smoothing and damping
    }
    
    public interface IPlayerHud
    {
        public void SetScheme(string controlScheme);
        public void SetGlyph(string actionId, Sprite icon);
        public void SetActive(bool active);
    }
    
    public interface IGlyphProvider
    {
        public Sprite GetGlyph(string controlScheme, string actionId);
        public Sprite GetGlyph(InputControl control); // Extra method for direct mapping
    }
    
    public interface IBindingStore
    {
        public void SaveBindings(string key, InputActionAsset actions);
        public void LoadBindings(string key, InputActionAsset actions);
        public void ClearBindings(string key);
    }
    
    public interface ISchemeMonitor : IDisposable
    {
        public event Action<string> SchemeChanged;
        
        public string CurrentScheme { get; }
        public void Attach(PlayerInput playerInput); // Subscribe to onControlsChanged etc.
        public void Detach();
    }
    
    // Sample additional feature as a new service
    // Used for storing power-ups, like the extra jumps in previous samples
    public interface IInventory
    {
        public event Action Changed;
        
        public int GetCount(ScriptableObject item);
        public void Add(ScriptableObject item, int amount = 1);
        public IReadOnlyDictionary<ScriptableObject,int> Items { get; }
    }
}
