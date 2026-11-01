
using UnityEngine;

public class LeverController : MonoBehaviour
{
    [SerializeField] private Animator leverAnimator;

    private static readonly int PullHash =
        Animator.StringToHash("Pull");

    public void PullLever()
    {
        if (leverAnimator == null)
            return;

        leverAnimator.SetTrigger(PullHash);
    }
}
