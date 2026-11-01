
public class SpinResult
{
    public SymbolData[] Symbols { get; }

    public SpinResult(SymbolData[] symbols)
    {
        Symbols = symbols;
    }

    public SymbolData GetSymbol(int index)
    {
        return Symbols[index];
    }

    public int ReelCount => Symbols.Length;
}
