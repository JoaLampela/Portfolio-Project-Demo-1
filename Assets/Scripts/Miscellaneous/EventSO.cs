using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSO", menuName = "Scriptable Objects/EventSO")]
public class EventSO : ScriptableObject
{
    private List<EventManager.EventListener> _eventListeners = new();

    public void AddListener(EventManager.EventListener listener) => _eventListeners.Add(listener);

    public void RemoveListener(EventManager.EventListener listener) => _eventListeners.Remove(listener);

    public void RaiseEvent() => _eventListeners.ForEach(listener => listener.OnEventRaised());
}
