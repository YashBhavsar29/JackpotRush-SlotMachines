
using System;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    [SerializeField, Min(0)]
    private int startingBalance = 1000;

    public int Balance { get; private set; }

    public event Action<int> OnBalanceChanged;

    private void Awake()
    {
        Balance = startingBalance;
        NotifyBalanceChanged();
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || Balance < amount)
            return false;

        Balance -= amount;
        NotifyBalanceChanged();

        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Balance += amount;
        NotifyBalanceChanged();
    }

    private void NotifyBalanceChanged()
    {
        OnBalanceChanged?.Invoke(Balance);
    }
}
