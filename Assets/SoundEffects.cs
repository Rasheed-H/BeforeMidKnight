using UnityEngine;
using UnityEngine.Audio;

public class SoundEffects : MonoBehaviour
{
    public static SoundEffects Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer; 

    private AudioSource audioSource;

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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        float savedVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.75f);
        SetVolume(savedVolume);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SoundEffectsVolume", Mathf.Log10(volume) * 20); 
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
    }

}
