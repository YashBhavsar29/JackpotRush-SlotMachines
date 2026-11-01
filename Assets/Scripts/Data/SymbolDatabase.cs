
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SymbolDatabase",
    menuName = "Slot Machine/Symbol Database"
)]
public class SymbolDatabase : ScriptableObject
{
    [Serializable]
    public class SymbolEntry
    {
        public SymbolData symbol;

        [Min(0)]
        public int occurrenceCount = 1;
    }

    [SerializeField] private List<SymbolEntry> symbols = new();

    public IReadOnlyList<SymbolEntry> Symbols => symbols;

    public int TotalOccurrences
    {
        get
        {
            int total = 0;

            foreach (SymbolEntry entry in symbols)
            {
                if (entry.symbol != null)
                    total += Mathf.Max(0, entry.occurrenceCount);
            }

            return total;
        }
    }

    public SymbolData GetRandomSymbol()
    {
        int total = TotalOccurrences;

        if (total <= 0)
            throw new InvalidOperationException(
                "SymbolDatabase has no valid symbol occurrences."
            );

        int randomValue = UnityEngine.Random.Range(0, total);

        foreach (SymbolEntry entry in symbols)
        {
            if (entry.symbol == null || entry.occurrenceCount <= 0)
                continue;

            randomValue -= entry.occurrenceCount;

            if (randomValue < 0)
                return entry.symbol;
        }

        throw new InvalidOperationException(
            "Unable to select a symbol."
        );
    }

    private void OnValidate()
    {
        if (TotalOccurrences <= 0)
            Debug.LogWarning(
                "SymbolDatabase needs at least one valid occurrence.",
                this
            );
    }
}
