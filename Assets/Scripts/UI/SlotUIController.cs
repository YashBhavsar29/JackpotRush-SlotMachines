
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text lastWinText;
    [SerializeField] private TMP_Text bestWinText;
    [SerializeField] private TMP_Text highBalanceText;
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private Button spinButton;

    [Header("Input Events")]
    [SerializeField] private VoidEventChannelSO spinRequested;
    [SerializeField] private IntEventChannelSO betChangeRequested;

    [Header("Output Events")]
    [SerializeField] private IntEventChannelSO balanceChanged;
    [SerializeField] private IntEventChannelSO betChanged;
    [SerializeField] private IntEventChannelSO lastWinChanged;
    [SerializeField] private IntEventChannelSO bestWinChanged;
    [SerializeField] private IntEventChannelSO highBalanceChanged;
    [SerializeField] private BoolEventChannelSO spinStateChanged;
    [SerializeField] private StringEventChannelSO messageChanged;

    private void OnEnable()
    {
        if (balanceChanged != null)
            balanceChanged.OnEventRaised += UpdateBalance;

        if (betChanged != null)
            betChanged.OnEventRaised += UpdateBet;

        if (lastWinChanged != null)
            lastWinChanged.OnEventRaised += UpdateLastWin;

        if (bestWinChanged != null)
            bestWinChanged.OnEventRaised += UpdateBestWin;

        if (highBalanceChanged != null)
            highBalanceChanged.OnEventRaised += UpdateHighBalance;

        if (spinStateChanged != null)
            spinStateChanged.OnEventRaised += SetSpinInteractable;

        if (messageChanged != null)
            messageChanged.OnEventRaised += ShowMessage;
    }

    private void OnDisable()
    {
        if (balanceChanged != null)
            balanceChanged.OnEventRaised -= UpdateBalance;

        if (betChanged != null)
            betChanged.OnEventRaised -= UpdateBet;

        if (lastWinChanged != null)
            lastWinChanged.OnEventRaised -= UpdateLastWin;

        if (bestWinChanged != null)
            bestWinChanged.OnEventRaised -= UpdateBestWin;

        if (highBalanceChanged != null)
            highBalanceChanged.OnEventRaised -= UpdateHighBalance;

        if (spinStateChanged != null)
            spinStateChanged.OnEventRaised -= SetSpinInteractable;

        if (messageChanged != null)
            messageChanged.OnEventRaised -= ShowMessage;
    }

    // UI Button callbacks

    public void OnSpinClicked()
    {
        spinRequested?.RaiseEvent();
    }

    public void IncreaseBet()
    {
        betChangeRequested?.RaiseEvent(1);
    }

    public void DecreaseBet()
    {
        betChangeRequested?.RaiseEvent(-1);
    }

    // Event listeners

    private void UpdateBalance(int value)
    {
        if (balanceText != null)
            balanceText.text = value.ToString();
    }

    private void UpdateBet(int value)
    {
        if (betText != null)
            betText.text = $"{value}G";
    }

    private void UpdateLastWin(int value)
    {
        if (lastWinText != null)
            lastWinText.text = value.ToString();
    }

    private void UpdateBestWin(int value)
    {
        if (bestWinText != null)
            bestWinText.text = value.ToString();
    }

    private void UpdateHighBalance(int value)
    {
        if (highBalanceText != null)
            highBalanceText.text = $"HIGH: {value}";
    }

    private void SetSpinInteractable(bool value)
    {
        if (spinButton != null)
            spinButton.interactable = value;
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }
}
