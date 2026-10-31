
using System.Collections.Generic;
using UnityEngine;

public class SpinResultGenerator : MonoBehaviour
{
    [SerializeField] private SymbolDatabase symbolDatabase;
    public SymbolDatabase SymbolDatabase => symbolDatabase;

    public SpinResult GenerateResult(int reelCount)
    {
        List<int> stopIndices = new List<int>();

        for (int i = 0; i < reelCount; i++)
        {
            int randomIndex = Random.Range(
                0,
                symbolDatabase.SymbolCount
            );

            stopIndices.Add(randomIndex);
        }

        return new SpinResult(stopIndices);
    }
}
