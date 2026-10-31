
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSymbol",
    menuName = "Slot Machine/Symbol Data"
)]
public class SymbolData : ScriptableObject
{
    [Header("Symbol Information")]
    [SerializeField] private string symbolId;
    [SerializeField] private string symbolName;
    [SerializeField] private Sprite symbolSprite;

    [Header("Gameplay Settings")]
    [SerializeField, Min(0.01f)]
    private float payoutMultiplier = 2f;

    [SerializeField, Min(0.01f)]
    private float probabilityWeight = 1f;

    public string SymbolId => symbolId;
    public string SymbolName => symbolName;
    public Sprite SymbolSprite => symbolSprite;
    public float PayoutMultiplier => payoutMultiplier;
    public float ProbabilityWeight => probabilityWeight;
}
