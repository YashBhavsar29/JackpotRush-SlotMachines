
using UnityEngine;

public class PayoutCalculator
{
    public int CalculatePayout(int bet, SymbolData symbol)
    {
        if (symbol == null || bet <= 0)
            return 0;

        return Mathf.RoundToInt(
            bet * symbol.PayoutMultiplier
        );
    }
}
