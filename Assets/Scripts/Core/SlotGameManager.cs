
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
    [SerializeField] private IntEventChannelSO betSelectionRequested;

    [Header("UI Events")]
    [SerializeField] private IntEventChannelSO betChanged;
    [SerializeField] private IntEventChannelSO lastWinChanged;
    [SerializeField] private IntEventChannelSO bestWinChanged;
    [SerializeField] private BoolEventChannelSO spinStateChanged;
    [SerializeField] private StringEventChannelSO messageChanged;
    [SerializeField] private SpinResultEventChannelSO spinResultCompleted;
    [SerializeField] private SpinResultGenerator resultGenerator;
    [Header("Gameplay Messages")]
    [SerializeField, Min(2f)]
    private float randomMessageMinInterval = 2.5f;

    [SerializeField, Min(2f)]
    private float randomMessageMaxInterval = 4f;
    private readonly WinEvaluator winEvaluator = new WinEvaluator();
    private readonly PayoutCalculator payoutCalculator =
        new PayoutCalculator();

    private int currentBet;
    private int lastWin;
    private int bestWin;

    public bool IsSpinning { get; private set; }
    public int CurrentBet => currentBet;
    private readonly string[] spinningMessages =
    {
        "GOOD LUCK!",
        "FEELING LUCKY?",
        "HERE WE GO!",
        "SPIN TO WIN!",
        "WAIT FOR IT..."
    };

    private readonly string[] tripleMatchMessages =
    {
        "FABULOUS!",
        "AWESOME!",
        "INCREDIBLE!",
        "AMAZING WIN!"
    };

    private readonly string[] dualMatchMessages =
    {
        "NICE ONE!",
        "GOOD CATCH!",
        "NOT BAD!"
    };

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

        if (betSelectionRequested != null)
            betSelectionRequested.OnEventRaised += SetBet;
    }

    private void Start()
    {
        if (!ValidateConfiguration())
            return;

        foreach (ReelController reel in reels)
            reel.Initialize();

        betChanged?.RaiseEvent(currentBet);
        lastWinChanged?.RaiseEvent(lastWin);
        bestWinChanged?.RaiseEvent(bestWin);
        spinStateChanged?.RaiseEvent(true);
    }

    private void OnDisable()
    {
        if (spinRequested != null)
            spinRequested.OnEventRaised -= Spin;

        if (betChangeRequested != null)
            betChangeRequested.OnEventRaised -= ChangeBet;

        if (betSelectionRequested != null)
            betSelectionRequested.OnEventRaised -= SetBet;
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

        // Random messages can appear while the reels are moving.
        Coroutine messageCoroutine = StartCoroutine(
            PlayRandomSpinningMessages()
        );

        // Wait for all reels to stop.
        for (int i = 0; i < reelCoroutines.Length; i++)
        {
            yield return reelCoroutines[i];
        }

        // No more random spinning messages after the reels stop.
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

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

            if (matchCount == 3)
            {
                messageChanged?.RaiseEvent(
                    $"TRIPLE MATCH! +{payout}"
                );

                // A random celebration message follows the result.
                string celebration = tripleMatchMessages[
                    Random.Range(0, tripleMatchMessages.Length)
                ];

                messageChanged?.RaiseEvent(celebration);
            }
            else
            {
                messageChanged?.RaiseEvent(
                    $"DUAL MATCH! +{payout}"
                );

                string celebration = dualMatchMessages[
                    Random.Range(0, dualMatchMessages.Length)
                ];

                messageChanged?.RaiseEvent(celebration);
            }

            audioManager?.PlayWin();
        }
        else
        {
            messageChanged?.RaiseEvent("NO MATCH!");
            messageChanged?.RaiseEvent("TRY AGAIN!");

            audioManager?.PlayLose();
        }

        lastWin = payout;
        bestWin = Mathf.Max(bestWin, payout);

        lastWinChanged?.RaiseEvent(lastWin);
        bestWinChanged?.RaiseEvent(bestWin);

        spinResultCompleted?.RaiseEvent(result);
    }
    private void SetBet(int amount)
    {
        if (IsSpinning) return;

        currentBet = Mathf.Clamp(amount, minimumBet, maximumBet);
        betChanged?.RaiseEvent(currentBet);
    }

    private IEnumerator PlayRandomSpinningMessages()
    {
        while (IsSpinning)
        {
            float waitTime = Random.Range(
                randomMessageMinInterval,
                randomMessageMaxInterval
            );

            yield return new WaitForSeconds(waitTime);

            if (!IsSpinning)
                yield break;

            string message = spinningMessages[
                Random.Range(0, spinningMessages.Length)
            ];

            messageChanged?.RaiseEvent(message);
        }
    }
}
