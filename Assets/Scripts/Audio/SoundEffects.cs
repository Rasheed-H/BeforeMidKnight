using UnityEngine;
using UnityEngine.Audio;

public class SoundEffects : MonoBehaviour
{
    public static SoundEffects Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer; 

    private AudioSource audioSource;

    /// <summary>
    /// Ensures a single instance of SoundEffects exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Initializes the AudioSource and sets the volume based on saved preferences.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        float savedVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.75f);
        SetVolume(savedVolume);
    }

    /// <summary>
    /// Plays the specified audio clip as a one-shot sound effect.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    public void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Sets the volume for sound effects and saves the preference.
    /// </summary>
    /// <param name="volume">The desired volume level (0.0001 to 1).</param>
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SoundEffectsVolume", Mathf.Log10(volume) * 20); 
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
    }

}
