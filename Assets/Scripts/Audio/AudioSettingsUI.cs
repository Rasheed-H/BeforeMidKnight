using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the audio settings UI, allowing the user to adjust music and sound effects volumes.
/// </summary>
public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;

    /// <summary>
    /// Initializes the sliders with saved volume settings and sets up listeners for volume changes.
    /// </summary>
    private void Start()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        soundEffectsVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.75f);

        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        soundEffectsVolumeSlider.onValueChanged.AddListener(SetSoundEffectsVolume);
    }

    /// <summary>
    /// Adjusts the music volume based on the slider value.
    /// </summary>
    /// <param name="value">The new music volume value.</param>
    private void SetMusicVolume(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
        }
    }

    /// <summary>
    /// Adjusts the sound effects volume based on the slider value.
    /// </summary>
    /// <param name="value">The new sound effects volume value.</param>
    private void SetSoundEffectsVolume(float value)
    {
        if (SoundEffects.Instance != null)
        {
            SoundEffects.Instance.SetVolume(value);
        }
    }
}
