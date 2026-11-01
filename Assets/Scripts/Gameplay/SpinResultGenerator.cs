
using System;
using UnityEngine;

public class SpinResultGenerator : MonoBehaviour
{
    [Header("Symbol Configuration")]
    [SerializeField] private SymbolDatabase symbolDatabase;

    public SymbolDatabase SymbolDatabase => symbolDatabase;

    public SpinResultGenerator(SymbolDatabase symbolDatabase)
    {
        this.symbolDatabase = symbolDatabase;
    }

    public SpinResult GenerateResult(int reelCount)
    {
        if (symbolDatabase == null)
        {
            throw new InvalidOperationException(
                "SymbolDatabase is not assigned to SpinResultGenerator."
            );
        }

        if (reelCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reelCount),
                "Reel count must be greater than zero."
            );
        }

        if (symbolDatabase.TotalOccurrences <= 0)
        {
            throw new InvalidOperationException(
                "SymbolDatabase contains no valid symbol occurrences."
            );
        }

        SymbolData[] results = new SymbolData[reelCount];

        for (int i = 0; i < reelCount; i++)
        {
            results[i] = symbolDatabase.GetRandomSymbol();
        }

        return new SpinResult(results);
    }
}
