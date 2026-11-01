
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private IntEventChannelSO balanceChanged;

    public int Balance { get; private set; }

    private void Awake()
    {
        Balance = startingBalance;
    }

    private void Start()
    {
        balanceChanged?.RaiseEvent(Balance);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || Balance < amount)
            return false;

        Balance -= amount;
        balanceChanged?.RaiseEvent(Balance);

        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Balance += amount;
        balanceChanged?.RaiseEvent(Balance);
    }
}
