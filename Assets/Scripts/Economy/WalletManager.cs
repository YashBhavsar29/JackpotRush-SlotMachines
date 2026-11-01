
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    [Header("Balance Settings")]
    [SerializeField] private int startingBalance = 1000;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO balanceChanged;
    [SerializeField] private IntEventChannelSO highBalanceChanged;

    private const string HighBalanceKey = "SlotMachine_HighBalance";

    public int Balance { get; private set; }
    public int HighBalance { get; private set; }

    private void Awake()
    {
        Balance = startingBalance;

        // Retrieve the previously saved high balance.
        HighBalance = PlayerPrefs.GetInt(
            HighBalanceKey,
            startingBalance
        );

        // Ensure the high balance is never below the starting balance.
        HighBalance = Mathf.Max(HighBalance, startingBalance);
    }

    private void Start()
    {
        balanceChanged?.RaiseEvent(Balance);
        highBalanceChanged?.RaiseEvent(HighBalance);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || Balance < amount)
            return false;

        Balance -= amount;

        UpdateBalance();
        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Balance += amount;

        UpdateBalance();
    }

    private void UpdateBalance()
    {
        balanceChanged?.RaiseEvent(Balance);

        if (Balance > HighBalance)
        {
            HighBalance = Balance;

            PlayerPrefs.SetInt(HighBalanceKey, HighBalance);
            PlayerPrefs.Save();

            highBalanceChanged?.RaiseEvent(HighBalance);
        }
    }

    public void ResetHighBalance()
    {
        HighBalance = startingBalance;

        PlayerPrefs.SetInt(HighBalanceKey, HighBalance);
        PlayerPrefs.Save();

        highBalanceChanged?.RaiseEvent(HighBalance);
    }
}
