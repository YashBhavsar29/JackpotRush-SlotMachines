
using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "VoidEvent",
    menuName = "Slot Machine/Events/Void Event"
)]
public class VoidEventChannelSO : ScriptableObject
{
    public event Action OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
