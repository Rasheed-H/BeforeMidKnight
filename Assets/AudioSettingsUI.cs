using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;

    private void Start()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        soundEffectsVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.75f);

        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        soundEffectsVolumeSlider.onValueChanged.AddListener(SetSoundEffectsVolume);
    }

    private void SetMusicVolume(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
        }
    }

    private void SetSoundEffectsVolume(float value)
    {
        if (SoundEffects.Instance != null)
        {
            SoundEffects.Instance.SetVolume(value);
        }
    }
}
