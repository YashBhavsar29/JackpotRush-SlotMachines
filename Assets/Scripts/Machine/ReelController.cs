
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

    [Header("Spin Settings")]
    [SerializeField, Min(0.01f)]
    private float symbolChangeInterval = 0.05f;

    private int currentCenterIndex;

    public bool IsSpinning { get; private set; }

    public void Initialize()
    {
        if (symbolDatabase == null ||
            symbolDatabase.SymbolCount == 0)
        {
            Debug.LogError(
                "Reel has no valid SymbolDatabase.",
                this
            );

            return;
        }

        currentCenterIndex = Random.Range(
            0,
            symbolDatabase.SymbolCount
        );

        UpdateVisibleSymbols();
    }

    public IEnumerator SpinTo(
        int targetIndex,
        float spinDuration,
        float startDelay
    )
    {
        if (IsSpinning)
            yield break;

        if (symbolDatabase == null ||
            symbolDatabase.SymbolCount == 0)
            yield break;

        IsSpinning = true;

        // Stagger the stopping of individual reels.
        yield return new WaitForSeconds(startDelay);

        targetIndex = symbolDatabase.WrapIndex(targetIndex);

        float elapsedTime = 0f;

        // Keep advancing through the fixed sequence.
        while (elapsedTime < spinDuration)
        {
            AdvanceOneStep();

            yield return new WaitForSeconds(
                symbolChangeInterval
            );

            elapsedTime += symbolChangeInterval;
        }

        // Continue forward until the predetermined
        // stopping position is reached.
        while (currentCenterIndex != targetIndex)
        {
            AdvanceOneStep();

            yield return new WaitForSeconds(
                symbolChangeInterval
            );
        }

        IsSpinning = false;
    }

    private void AdvanceOneStep()
    {
        currentCenterIndex = symbolDatabase.WrapIndex(
            currentCenterIndex + 1
        );

        UpdateVisibleSymbols();
    }

    private void UpdateVisibleSymbols()
    {
        topSymbol.SetSymbol(
            symbolDatabase.GetSymbolAt(currentCenterIndex - 1)
        );

        middleSymbol.SetSymbol(
            symbolDatabase.GetSymbolAt(currentCenterIndex)
        );

        bottomSymbol.SetSymbol(
            symbolDatabase.GetSymbolAt(currentCenterIndex + 1)
        );
    }
}
