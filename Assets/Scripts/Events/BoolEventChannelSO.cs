
using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BoolEvent",
    menuName = "Slot Machine/Events/Bool Event"
)]
public class BoolEventChannelSO : ScriptableObject
{
    public event Action<bool> OnEventRaised;

    public void RaiseEvent(bool value)
    {
        OnEventRaised?.Invoke(value);
    }
}
