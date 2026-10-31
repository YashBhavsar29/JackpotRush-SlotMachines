
using System.Collections;
using UnityEngine;

public class SlotGameManager : MonoBehaviour
{
    public static SlotGameManager Instance { get; private set; }

    [Header("Gameplay References")]
    [SerializeField] private ReelController[] reels;
    [SerializeField] private SpinResultGenerator resultGenerator;
    [SerializeField] private WalletManager walletManager;
    [SerializeField] private LeverController leverController;
    [SerializeField] private SlotUIController uiController;

    [Header("Spin Settings")]
    [SerializeField, Min(0.1f)]
    private float spinDuration = 2f;

    [SerializeField, Min(0f)]
    private float reelStopDelay = 0.4f;

    [SerializeField, Min(0f)]
    private float leverToReelDelay = 0.35f;

    [Header("Bet Settings")]
    [SerializeField, Min(1)]
    private int currentBet = 10;

    private bool isSpinning;

    private WinEvaluator winEvaluator;

    private readonly PayoutCalculator payoutCalculator =
        new PayoutCalculator();

    public int CurrentBet => currentBet;
    public bool IsSpinning => isSpinning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (reels == null || reels.Length != 3)
        {
            Debug.LogError(
                "SlotGameManager requires exactly three reels.",
                this
            );

            enabled = false;
            return;
        }

        if (resultGenerator == null || walletManager == null)
        {
            Debug.LogError(
                "Missing required gameplay references.",
                this
            );

            enabled = false;
            return;
        }

        if (resultGenerator.SymbolDatabase == null)
        {
            Debug.LogError("SymbolDatabase is missing.", this);
            enabled = false;
            return;
        }

        foreach (ReelController reel in reels)
        {
            if (reel == null)
            {
                Debug.LogError("A ReelController reference is missing.");
                enabled = false;
                return;
            }

            reel.Initialize();
        }

        winEvaluator = new WinEvaluator(
            resultGenerator.SymbolDatabase
        );

        uiController?.UpdateBet(currentBet);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetBet(int amount)
    {
        if (isSpinning || amount <= 0)
            return;

        currentBet = amount;

        uiController?.UpdateBet(currentBet);
    }

    public void Spin()
    {
        if (isSpinning)
            return;

        if (reels == null || reels.Length != 3)
            return;

        if (!walletManager.TrySpend(currentBet))
        {
            uiController?.ShowMessage("Insufficient balance!");
            AudioManager.Instance?.PlayError();
            return;
        }

        // Determine the final result before animation.
        SpinResult result =
            resultGenerator.GenerateResult(reels.Length);

        StartCoroutine(PlaySpin(result));
    }

    private IEnumerator PlaySpin(SpinResult result)
    {
        isSpinning = true;

        uiController?.SetSpinInteractable(false);
        uiController?.ShowMessage("Spinning...");

        // Pull lever and play the complete spin sound.
        leverController?.PullLever();
        AudioManager.Instance?.PlayLeverPull();

        // Wait before starting the reels.
        yield return new WaitForSeconds(leverToReelDelay);

        // Start all reels with staggered stopping delays.
        Coroutine[] reelCoroutines =
            new Coroutine[reels.Length];

        for (int i = 0; i < reels.Length; i++)
        {
            float stopDelay = i * reelStopDelay;

            reelCoroutines[i] = StartCoroutine(
                reels[i].SpinTo(
                    result.GetStopIndex(i),
                    spinDuration,
                    stopDelay
                )
            );
        }

        // Wait until all reels have stopped.
        for (int i = 0; i < reelCoroutines.Length; i++)
        {
            yield return reelCoroutines[i];
        }

        EvaluateResult(result);

        isSpinning = false;

        uiController?.SetSpinInteractable(true);
    }

    private void EvaluateResult(SpinResult result)
    {
        if (winEvaluator.TryEvaluate(
            result,
            out SymbolData winningSymbol))
        {
            int payout = payoutCalculator.CalculatePayout(
                currentBet,
                winningSymbol
            );

            walletManager.AddCoins(payout);

            uiController?.ShowMessage(
                $"YOU WIN! +{payout}"
            );

            AudioManager.Instance?.PlayWin();
        }
        else
        {
            uiController?.ShowMessage("Try Again!");
            AudioManager.Instance?.PlayLose();
        }
    }
}
