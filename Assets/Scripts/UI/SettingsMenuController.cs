using Unity.VisualScripting;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Settings Menu Components")]
    public GameObject SettingsMenu;
    public GameObject SettingsOptions;
    public GameObject ControlsSettings;
    public GameObject AudioSettings;
    private int page = 0;

    /// <summary>
    /// Enables the Settings Menu.
    /// </summary>
    public void SettingsMenuPressed()
    {
        page = 1;
        SettingsMenu.SetActive(true);
        SettingsOptions.SetActive(true);
        ControlsSettings.SetActive(false);
        AudioSettings.SetActive(false);
    }

    /// <summary>
    /// Enables Keyboard Settings and disables Settings Options.
    /// </summary>
    public void ControlsSettingsPressed()
    {
        page = 2;
        SettingsOptions.SetActive(false);
        ControlsSettings.SetActive(true);
    }

    /// <summary>
    /// Enables Audio Settings and disables Settings Options.
    /// </summary>
    public void AudioSettingsPressed()
    {
        page = 2;
        SettingsOptions.SetActive(false);
        AudioSettings.SetActive(true);
    }

    /// <summary>
    /// Disables Keyboard, Gamepad, and Audio Settings and enables Settings Options.
    /// </summary>
    public void BackButtonPressed()
    {
        if (page == 2)
        {
            SettingsBack();
        }
        else
        {
            SettingsClose();
        }
    }

    public void SettingsBack()
    {
        page = 1;
        ControlsSettings.SetActive(false);
        AudioSettings.SetActive(false);
        SettingsOptions.SetActive(true);
    }

    /// <summary>
    /// Closes the Settings Menu.
    /// </summary>
    public void SettingsClose()
    {
        SettingsMenu.SetActive(false);
    }
}
