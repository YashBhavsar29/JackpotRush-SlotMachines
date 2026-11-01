
using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SpinResultEvent",
    menuName = "Slot Machine/Events/Spin Result Event"
)]
public class SpinResultEventChannelSO : ScriptableObject
{
    public event Action<SpinResult> OnEventRaised;

    public void RaiseEvent(SpinResult result)
    {
        OnEventRaised?.Invoke(result);
    }
}
