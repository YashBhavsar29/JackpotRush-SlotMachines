
public class WinEvaluator
{
    private readonly SymbolDatabase symbolDatabase;

    public WinEvaluator(SymbolDatabase database)
    {
        symbolDatabase = database;
    }

    public bool TryEvaluate(
        SpinResult result,
        out SymbolData winningSymbol
    )
    {
        winningSymbol = null;

        if (result == null || result.ReelCount < 2)
            return false;

        SymbolData firstSymbol = symbolDatabase.GetSymbolAt(
            result.GetStopIndex(0)
        );

        for (int i = 1; i < result.ReelCount; i++)
        {
            SymbolData currentSymbol = symbolDatabase.GetSymbolAt(
                result.GetStopIndex(i)
            );

            if (currentSymbol != firstSymbol)
                return false;
        }

        winningSymbol = firstSymbol;
        return true;
    }
}
