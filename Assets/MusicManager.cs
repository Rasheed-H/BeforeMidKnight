using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer; 
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is missing on MusicManager!");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        SetVolume(savedVolume);
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
            return;
        }

        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

}
