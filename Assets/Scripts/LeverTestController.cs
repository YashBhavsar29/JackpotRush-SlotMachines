
using UnityEngine;
using UnityEngine.InputSystem;

public class LeverTestController : MonoBehaviour
{
    [Header("Lever References")]
    [SerializeField] private Animator leverAnimator;

    [Header("Audio References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip leverPullSound;

    [Header("Input Settings")]
    [SerializeField] private Key testKey = Key.Space;

    private static readonly int PullHash =
        Animator.StringToHash("Pull");

    private bool isPulling;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[testKey].wasPressedThisFrame && !isPulling)
        {
            PullLever();
        }
    }

    private void PullLever()
    {
        if (leverAnimator == null)
        {
            Debug.LogWarning("Lever Animator is not assigned!");
            return;
        }

        isPulling = true;
        leverAnimator.SetTrigger(PullHash);

        if (audioSource != null && leverPullSound != null)
        {
            audioSource.PlayOneShot(leverPullSound);
        }

        CancelInvoke(nameof(ResetLever));
        Invoke(nameof(ResetLever), 0.6f);
    }

    private void ResetLever()
    {
        isPulling = false;
    }
}
