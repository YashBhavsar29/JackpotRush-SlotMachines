
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip spinClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip errorClip;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlaySpin() => PlayClip(spinClip);
    public void PlayWin() => PlayClip(winClip);
    public void PlayLose() => PlayClip(loseClip);
    public void PlayError() => PlayClip(errorClip);

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}
