
using System.Collections;
using UnityEngine;

public class SlotGameManager : MonoBehaviour
{
    public static SlotGameManager Instance { get; private set; }

    [Header("Gameplay Components")]
    [SerializeField] private ReelController[] reels;
    [SerializeField] private WalletManager walletManager;
    [SerializeField] private LeverController leverController;
    [SerializeField] private AudioManager audioManager;

    [Header("Configuration")]
    [SerializeField] private SymbolDatabase symbolDatabase;
    [SerializeField, Min(0.1f)] private float spinDuration = 2f;
    [SerializeField, Min(0f)] private float reelStopDelay = 0.4f;
    [SerializeField, Min(0f)] private float leverToReelDelay = 0.35f;
    [SerializeField] private int startingBet = 10;
    [SerializeField] private int minimumBet = 10;
    [SerializeField] private int maximumBet = 100;
    [SerializeField] private int betIncrement = 10;

    [Header("Input Events")]
    [SerializeField] private VoidEventChannelSO spinRequested;
    [SerializeField] private IntEventChannelSO betChangeRequested;

    [Header("UI Events")]
    [SerializeField] private IntEventChannelSO betChanged;
    [SerializeField] private IntEventChannelSO lastWinChanged;
    [SerializeField] private IntEventChannelSO bestWinChanged;
    [SerializeField] private IntEventChannelSO highBalanceChanged;
    [SerializeField] private BoolEventChannelSO spinStateChanged;
    [SerializeField] private StringEventChannelSO messageChanged;
    [SerializeField] private SpinResultEventChannelSO spinResultCompleted;

    [SerializeField] private SpinResultGenerator resultGenerator;
    private readonly WinEvaluator winEvaluator = new WinEvaluator();
    private readonly PayoutCalculator payoutCalculator =
        new PayoutCalculator();

    private int currentBet;
    private int lastWin;
    private int bestWin;
    private int highBalance;

    public bool IsSpinning { get; private set; }
    public int CurrentBet => currentBet;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentBet = startingBet;
    }

    private void OnEnable()
    {
        if (spinRequested != null)
            spinRequested.OnEventRaised += Spin;

        if (betChangeRequested != null)
            betChangeRequested.OnEventRaised += ChangeBet;
    }

    private void Start()
    {
        if (!ValidateConfiguration())
            return;

        foreach (ReelController reel in reels)
            reel.Initialize();

        highBalance = walletManager.Balance;

        betChanged?.RaiseEvent(currentBet);
        lastWinChanged?.RaiseEvent(lastWin);
        bestWinChanged?.RaiseEvent(bestWin);
        highBalanceChanged?.RaiseEvent(highBalance);
        spinStateChanged?.RaiseEvent(true);
    }

    private void OnDisable()
    {
        if (spinRequested != null)
            spinRequested.OnEventRaised -= Spin;

        if (betChangeRequested != null)
            betChangeRequested.OnEventRaised -= ChangeBet;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private bool ValidateConfiguration()
    {
        if (reels == null || reels.Length != 3 ||
            walletManager == null || symbolDatabase == null ||
            symbolDatabase.TotalOccurrences <= 0)
        {
            Debug.LogError(
                "SlotGameManager has an invalid configuration.",
                this
            );

            enabled = false;
            return false;
        }

        foreach (ReelController reel in reels)
        {
            if (reel == null)
            {
                Debug.LogError("A reel reference is missing.", this);
                enabled = false;
                return false;
            }
        }

        if (resultGenerator == null)
        {
            Debug.LogError("Result generator could not be initialized.", this);
            enabled = false;
            return false;
        }

        return true;
    }

    private void ChangeBet(int direction)
    {
        if (IsSpinning)
            return;

        currentBet = Mathf.Clamp(
            currentBet + direction * betIncrement,
            minimumBet,
            maximumBet
        );

        betChanged?.RaiseEvent(currentBet);
    }

    private void Spin()
    {
        if (IsSpinning)
            return;

        if (!walletManager.TrySpend(currentBet))
        {
            messageChanged?.RaiseEvent("INSUFFICIENT BALANCE");
            audioManager?.PlayError();
            return;
        }

        SpinResult result =
            resultGenerator.GenerateResult(reels.Length);

        StartCoroutine(PlaySpin(result));
    }

    private IEnumerator PlaySpin(SpinResult result)
    {
        IsSpinning = true;

        spinStateChanged?.RaiseEvent(false);
        messageChanged?.RaiseEvent("SPINNING...");

        leverController?.PullLever();
        audioManager?.PlaySpin();

        yield return new WaitForSeconds(leverToReelDelay);

        Coroutine[] reelCoroutines = new Coroutine[reels.Length];

        for (int i = 0; i < reels.Length; i++)
        {
            reelCoroutines[i] = StartCoroutine(
                reels[i].SpinTo(
                    result.GetSymbol(i),
                    spinDuration,
                    i * reelStopDelay
                )
            );
        }

        for (int i = 0; i < reelCoroutines.Length; i++)
            yield return reelCoroutines[i];

        EvaluateResult(result);

        IsSpinning = false;
        spinStateChanged?.RaiseEvent(true);
    }


    private void EvaluateResult(SpinResult result)
    {
        int payout = 0;

        if (winEvaluator.TryEvaluate(
            result,
            out SymbolData winningSymbol,
            out int matchCount))
        {
            payout = payoutCalculator.CalculatePayout(
                currentBet,
                winningSymbol,
                matchCount
            );

            walletManager.AddCoins(payout);

            messageChanged?.RaiseEvent(
                $"YOU WIN! {winningSymbol.SymbolName} x{matchCount} +{payout}"
            );

            audioManager?.PlayWin();
        }
        else
        {
            messageChanged?.RaiseEvent("TRY AGAIN!");
            audioManager?.PlayLose();
        }

        lastWin = payout;
        bestWin = Mathf.Max(bestWin, payout);
        highBalance = Mathf.Max(highBalance, walletManager.Balance);

        lastWinChanged?.RaiseEvent(lastWin);
        bestWinChanged?.RaiseEvent(bestWin);
        highBalanceChanged?.RaiseEvent(highBalance);

        spinResultCompleted?.RaiseEvent(result);
    }
}
