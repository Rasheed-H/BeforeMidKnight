using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the cooldown display for abilities, including UI updates for fill amount, remaining time text,
/// and rotating edge effect to visually represent the cooldown timer.
/// </summary>
public class AbilityCooldownDisplay : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image imageCooldown;
    [SerializeField] private TMP_Text textCooldown;
    [SerializeField] private Image imageEdge;

    private float cooldownTime;
    private float cooldownTimer;
    private bool isCooldownActive;

    /// <summary>
    /// Initializes the cooldown UI by hiding any active cooldown effects.
    /// </summary>
    private void Start()
    {
        ResetCooldownUI();
    }

    /// <summary>
    /// Updates the cooldown timer if active, applying visual changes each frame.
    /// </summary>
    private void Update()
    {
        if (isCooldownActive)
        {
            ApplyCooldown();
        }
    }

    /// <summary>
    /// Starts the cooldown sequence with the specified duration, activating the UI elements.
    /// </summary>
    /// <param name="abilityCooldown">Duration of the cooldown for the ability.</param>
    public void StartCooldown(float abilityCooldown)
    {
        if (isCooldownActive) return;

        cooldownTime = abilityCooldown;
        cooldownTimer = cooldownTime;
        isCooldownActive = true;

        textCooldown.gameObject.SetActive(true);
        textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
        imageCooldown.fillAmount = 1.0f;
        imageEdge.gameObject.SetActive(true);
    }

    /// <summary>
    /// Applies updates to the cooldown visuals, adjusting the fill amount, edge rotation,
    /// and remaining cooldown text based on the elapsed time.
    /// </summary>
    private void ApplyCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            ResetCooldownUI();
        }
        else
        {
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
            imageEdge.transform.localEulerAngles = new Vector3(0, 0, 360.0f * (cooldownTimer / cooldownTime));

            if (cooldownTimer < 1f)
            {
                textCooldown.text = cooldownTimer.ToString("F1");
            }
            else
            {
                textCooldown.text = Mathf.CeilToInt(cooldownTimer).ToString();
            }
        }
    }

    /// <summary>
    /// Resets the cooldown UI elements to their default states when the cooldown finishes.
    /// </summary>
    private void ResetCooldownUI()
    {
        isCooldownActive = false;
        textCooldown.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0.0f;
        imageEdge.gameObject.SetActive(false);
    }
}
