
using System.Collections;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("Symbol References")]
    [SerializeField] private ReelSymbolView topSymbol;
    [SerializeField] private ReelSymbolView middleSymbol;
    [SerializeField] private ReelSymbolView bottomSymbol;

    [Header("Symbol Configuration")]
    [SerializeField] private SymbolDatabase symbolDatabase;

    [Header("Animation Settings")]
    [SerializeField, Min(0.01f)]
    private float symbolHeight = 1f;

    [SerializeField, Min(0.01f)]
    private float symbolChangeInterval = 0.08f;

    public bool IsSpinning { get; private set; }

    public void Initialize()
    {
        topSymbol.SetSymbol(symbolDatabase.GetRandomSymbol());
        middleSymbol.SetSymbol(symbolDatabase.GetRandomSymbol());
        bottomSymbol.SetSymbol(symbolDatabase.GetRandomSymbol());
    }

    public IEnumerator SpinTo(
        SymbolData finalSymbol,
        float spinDuration,
        float startDelay
    )
    {
        if (IsSpinning || finalSymbol == null)
            yield break;

        IsSpinning = true;

        yield return new WaitForSeconds(startDelay);

        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            yield return AnimateOneStep();

            elapsed += symbolChangeInterval;
        }

        // The predetermined result is always displayed in the centre.
        middleSymbol.SetSymbol(finalSymbol);

        IsSpinning = false;
    }

    private IEnumerator AnimateOneStep()
    {
        ReelSymbolView oldTop = topSymbol;
        ReelSymbolView oldMiddle = middleSymbol;
        ReelSymbolView oldBottom = bottomSymbol;

        Vector3 topPosition = oldTop.transform.localPosition;
        Vector3 middlePosition = oldMiddle.transform.localPosition;
        Vector3 bottomPosition = oldBottom.transform.localPosition;

        Vector3 targetTop = topPosition + Vector3.down * symbolHeight;
        Vector3 targetMiddle = middlePosition + Vector3.down * symbolHeight;
        Vector3 targetBottom = bottomPosition + Vector3.down * symbolHeight;

        float elapsed = 0f;

        while (elapsed < symbolChangeInterval)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / symbolChangeInterval);

            oldTop.transform.localPosition =
                Vector3.Lerp(topPosition, targetTop, t);

            oldMiddle.transform.localPosition =
                Vector3.Lerp(middlePosition, targetMiddle, t);

            oldBottom.transform.localPosition =
                Vector3.Lerp(bottomPosition, targetBottom, t);

            yield return null;
        }

        // Wrap the bottom symbol to the top.
        oldBottom.transform.localPosition = topPosition;

        // Assign a new random symbol as it re-enters.
        oldBottom.SetSymbol(symbolDatabase.GetRandomSymbol());

        // Rotate the references to match their new positions.
        topSymbol = oldBottom;
        middleSymbol = oldTop;
        bottomSymbol = oldMiddle;

        // Correct final positions to avoid accumulated drift.
        topSymbol.transform.localPosition = topPosition;
        middleSymbol.transform.localPosition = middlePosition;
        bottomSymbol.transform.localPosition = bottomPosition;
    }
}
