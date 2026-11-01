
using UnityEngine;

public class PayoutCalculator
{
    public int CalculatePayout(
        int bet,
        SymbolData symbol,
        int matchCount)
    {
        if (bet <= 0 || symbol == null)
            return 0;

        float multiplier = symbol.GetMultiplier(matchCount);

        if (multiplier <= 0f)
            return 0;

        return Mathf.RoundToInt(bet * multiplier);
    }
}
