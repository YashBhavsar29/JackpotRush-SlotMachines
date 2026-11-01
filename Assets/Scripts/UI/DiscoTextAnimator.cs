
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DiscoTextAnimator : MonoBehaviour
{
    [Header("Color Animation")]
    [SerializeField, Min(0.01f)]
    private float colorCycleSpeed = 0.25f;

    [SerializeField, Range(0f, 1f)]
    private float saturation = 1f;

    [SerializeField, Range(0f, 1f)]
    private float brightness = 1f;

    [Header("Brightness Pulse")]
    [SerializeField]
    private bool enablePulse = true;

    [SerializeField, Min(0f)]
    private float pulseSpeed = 2f;

    [SerializeField, Range(0f, 0.5f)]
    private float pulseAmount = 0.15f;

    private TMP_Text targetText;
    private float hue;

    private void Awake()
    {
        targetText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        AnimateColor();
    }

    private void AnimateColor()
    {
        // Continuously cycle through the complete color spectrum.
        hue += colorCycleSpeed * Time.unscaledDeltaTime;
        hue %= 1f;

        float currentBrightness = brightness;

        // Subtle brightness pulse for a neon display effect.
        if (enablePulse)
        {
            float pulse = Mathf.Sin(
                Time.unscaledTime * pulseSpeed
            );

            currentBrightness *= 1f - pulseAmount * (0.5f + 0.5f * pulse);
        }

        targetText.color = Color.HSVToRGB(
            hue,
            saturation,
            currentBrightness
        );
    }
}
