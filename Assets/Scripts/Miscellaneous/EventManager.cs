#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [SerializeField] private List<EventListener> _events = new();
    
    private void OnEnable()
    {
        _events.ForEach(eventAction => eventAction.Enable());
    }
    
    private void OnDisable()
    {
        _events.ForEach(eventAction => eventAction.Disable());
    }
    
    // Should probably be in Joa.Models but is here for pedagogic reasons
    [Serializable]
    public class EventListener
    {
        public EventSO? eventSO;
        public UnityEvent? action;
        
        public void Enable()
        {
            eventSO?.AddListener(this);
        }
        
        public void Disable()
        {
            eventSO?.RemoveListener(this);
        }
        
        public void OnEventRaised()
        {
            action?.Invoke();
        }
    }
}
