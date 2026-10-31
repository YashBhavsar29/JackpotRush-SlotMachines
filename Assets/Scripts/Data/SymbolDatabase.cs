
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SymbolDatabase",
    menuName = "Slot Machine/Symbol Database"
)]
public class SymbolDatabase : ScriptableObject
{
    [Header("Fixed Reel Symbol Sequence")]
    [SerializeField] private List<SymbolData> symbols = new();

    public int SymbolCount => symbols.Count;

    public SymbolData GetSymbolAt(int index)
    {
        if (symbols == null || symbols.Count == 0)
            throw new InvalidOperationException(
                "Symbol database is empty."
            );

        return symbols[WrapIndex(index)];
    }

    public int WrapIndex(int index)
    {
        return (index % symbols.Count + symbols.Count) % symbols.Count;
    }

    private void OnValidate()
    {
        if (symbols == null)
            return;

        for (int i = 0; i < symbols.Count; i++)
        {
            if (symbols[i] == null)
            {
                Debug.LogWarning(
                    $"Symbol at index {i} is missing.",
                    this
                );
            }
        }
    }
}
