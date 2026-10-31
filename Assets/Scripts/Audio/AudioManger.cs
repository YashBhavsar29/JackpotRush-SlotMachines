
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Slot Machine Sounds")]
    [SerializeField] private AudioClip leverPullClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip errorClip;

    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayLeverPull()
    {
        PlaySFX(leverPullClip);
    }

    public void PlayWin()
    {
        PlaySFX(winClip);
    }

    public void PlayLose()
    {
        PlaySFX(loseClip);
    }

    public void PlayError()
    {
        PlaySFX(errorClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.PlayOneShot(clip, masterVolume);
    }
}
