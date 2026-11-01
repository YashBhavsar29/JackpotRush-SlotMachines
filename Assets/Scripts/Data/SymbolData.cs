
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSymbol",
    menuName = "Slot Machine/Symbol Data"
)]
public class SymbolData : ScriptableObject
{
    [Header("Symbol Information")]
    [SerializeField] private string symbolName;
    [SerializeField] private Sprite symbolSprite;

    [Header("Payout Multipliers")]
    [SerializeField, Min(0f)]
    private float pairMultiplier = 1f;

    [SerializeField, Min(0f)]
    private float tripleMultiplier = 5f;

    public string SymbolName => symbolName;
    public Sprite SymbolSprite => symbolSprite;

    public float PairMultiplier => pairMultiplier;
    public float TripleMultiplier => tripleMultiplier;

    public float GetMultiplier(int matchCount)
    {
        return matchCount switch
        {
            2 => pairMultiplier,
            3 => tripleMultiplier,
            _ => 0f
        };
    }
}
