
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ReelSymbolView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public SymbolData CurrentSymbol { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSymbol(SymbolData symbol)
    {
        if (symbol == null)
            return;

        CurrentSymbol = symbol;
        spriteRenderer.sprite = symbol.SymbolSprite;
    }
}
