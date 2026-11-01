
public class WinEvaluator
{
    public bool TryEvaluate(
        SpinResult result,
        out SymbolData winningSymbol,
        out int matchCount)
    {
        winningSymbol = null;
        matchCount = 0;

        if (result == null || result.ReelCount != 3)
            return false;

        SymbolData first = result.GetSymbol(0);
        SymbolData second = result.GetSymbol(1);
        SymbolData third = result.GetSymbol(2);

        // Check three matching symbols first.
        if (first == second && second == third)
        {
            winningSymbol = first;
            matchCount = 3;
            return true;
        }

        // Check for exactly two matching symbols.
        if (first == second)
            winningSymbol = first;
        else if (first == third)
            winningSymbol = first;
        else if (second == third)
            winningSymbol = second;

        if (winningSymbol != null)
        {
            matchCount = 2;
            return true;
        }

        return false;
    }
}
