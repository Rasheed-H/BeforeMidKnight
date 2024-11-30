using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Settings Menu Components")]
    public GameObject SettingsMenu;
    public GameObject SettingsOptions;
    public GameObject KeyboardSettings;
    public GameObject GamepadSettings;
    public GameObject AudioSettings;

    /// <summary>
    /// Enables the Settings Menu.
    /// </summary>
    public void SettingsMenuPressed()
    {
        SettingsMenu.SetActive(true);
        SettingsOptions.SetActive(true);
        KeyboardSettings.SetActive(false);
        GamepadSettings.SetActive(false);
        AudioSettings.SetActive(false);
    }

    /// <summary>
    /// Enables Keyboard Settings and disables Settings Options.
    /// </summary>
    public void KeyboardSettingsPressed()
    {
        SettingsOptions.SetActive(false);
        KeyboardSettings.SetActive(true);
    }

    /// <summary>
    /// Enables Gamepad Settings and disables Settings Options.
    /// </summary>
    public void GamepadSettingsPressed()
    {
        SettingsOptions.SetActive(false);
        GamepadSettings.SetActive(true);
    }

    /// <summary>
    /// Enables Audio Settings and disables Settings Options.
    /// </summary>
    public void AudioSettingsPressed()
    {
        SettingsOptions.SetActive(false);
        AudioSettings.SetActive(true);
    }

    /// <summary>
    /// Disables Keyboard, Gamepad, and Audio Settings and enables Settings Options.
    /// </summary>
    public void BackButtonPressed()
    {
        KeyboardSettings.SetActive(false);
        GamepadSettings.SetActive(false);
        AudioSettings.SetActive(false);
        SettingsOptions.SetActive(true);
    }

    /// <summary>
    /// Closes the Settings Menu.
    /// </summary>
    public void CloseButtonPressed()
    {
        SettingsMenu.SetActive(false);
    }
}
