
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlotGameManager gameManager;
    [SerializeField] private WalletManager walletManager;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button spinButton;

    private void OnEnable()
    {
        if (walletManager != null)
        {
            walletManager.OnBalanceChanged += UpdateBalance;
        }
    }

    private void Start()
    {
        UpdateBalance(walletManager.Balance);
        UpdateBet(gameManager.CurrentBet);
    }

    private void OnDisable()
    {
        if (walletManager != null)
        {
            walletManager.OnBalanceChanged -= UpdateBalance;
        }
    }

    public void UpdateBalance(int balance)
    {
        if (balanceText != null)
            balanceText.text = $"Balance: {balance}";
    }

    public void UpdateBet(int amount)
    {
        if (betText != null)
            betText.text = $"Bet: {amount}";
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    public void SetSpinInteractable(bool interactable)
    {
        if (spinButton != null)
            spinButton.interactable = interactable;
    }

    // Assign these methods to your UI buttons.

    public void SelectBet10()
    {
        gameManager.SetBet(10);
    }

    public void SelectBet50()
    {
        gameManager.SetBet(50);
    }

    public void SelectBet100()
    {
        gameManager.SetBet(100);
    }

    public void OnSpinButtonClicked()
    {
        gameManager.Spin();
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
