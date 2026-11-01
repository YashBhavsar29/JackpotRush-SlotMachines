
using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "StringEvent",
    menuName = "Slot Machine/Events/String Event"
)]
public class StringEventChannelSO : ScriptableObject
{
    public event Action<string> OnEventRaised;

    public void RaiseEvent(string value)
    {
        OnEventRaised?.Invoke(value);
    }
}
