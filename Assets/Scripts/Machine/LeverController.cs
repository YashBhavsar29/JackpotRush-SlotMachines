
using System.Collections;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public static LeverController Instance { get; private set; }

    [Header("Animation")]
    [SerializeField] private Animator leverAnimator;

    [SerializeField, Min(0.1f)]
    private float pullAnimationDuration = 0.6f;

    private static readonly int PullHash =
        Animator.StringToHash("Pull");

    public bool IsPulling { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PullLever()
    {
        if (IsPulling)
            return;

        if (leverAnimator == null)
        {
            Debug.LogWarning("Lever Animator is not assigned.");
            return;
        }

        IsPulling = true;

        leverAnimator.SetTrigger(PullHash);

        AudioManager.Instance?.PlayLeverPull();

        StartCoroutine(ResetLeverState());
    }

    private IEnumerator ResetLeverState()
    {
        yield return new WaitForSeconds(pullAnimationDuration);

        IsPulling = false;
    }
}
