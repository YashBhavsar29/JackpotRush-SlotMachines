
using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "IntEvent",
    menuName = "Slot Machine/Events/Int Event"
)]
public class IntEventChannelSO : ScriptableObject
{
    public event Action<int> OnEventRaised;

    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}
