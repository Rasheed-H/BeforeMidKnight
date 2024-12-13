using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Singleton class to manage background music in the game, including playback, stopping music, 
/// and adjusting volume. Persists across scenes.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer; 
    private AudioSource audioSource;

    /// <summary>
    /// Initializes the `MusicManager` as a singleton instance and ensures it persists across scenes.
    /// Verifies the presence of an `AudioSource` component.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Loads the saved music volume from player preferences and applies it to the audio mixer.
    /// If no preference is found, sets the volume to a default value of 0.75.
    /// </summary>
    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        SetVolume(savedVolume);
    }

    /// <summary>
    /// Starts playing a new music track. The track loops continuously.
    /// </summary>
    /// <param name="newClip">The AudioClip to be played as background music.</param>
    public void PlayMusic(AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// Stops the currently playing music, if any.
    /// </summary>
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Adjusts the volume of the music and saves the setting for future sessions.
    /// </summary>
    /// <param name="volume">The desired volume level, clamped between 0.0001 and 1.</param>
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

}
