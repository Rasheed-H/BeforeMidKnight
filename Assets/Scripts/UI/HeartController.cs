using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the display of a heart UI element, allowing it to represent full, half, or empty status.
/// </summary>
public class HeartController : MonoBehaviour
{
    public Sprite fullHeartSprite;
    public Sprite halfHeartSprite;
    public Sprite emptyHeartSprite;

    private Image heartImage;

    /// <summary>
    /// Initializes the heart's Image component.
    /// </summary>
    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    /// <summary>
    /// Sets the heart's sprite based on its health status (full, half, empty).
    /// </summary>
    public void SetHeartImage(HeartStatus heartStatus)
    {
        switch (heartStatus)
        {
            case HeartStatus.Full:
                heartImage.sprite = fullHeartSprite;
                break;
            case HeartStatus.Half:
                heartImage.sprite = halfHeartSprite;
                break;
            case HeartStatus.Empty:
                heartImage.sprite = emptyHeartSprite;
                break;
        }
    }
}

/// <summary>
/// Enum representing the possible states of a heart: Full, Half, or Empty.
/// </summary>
public enum HeartStatus
{
    Empty = 0,
    Half = 1,
    Full = 2
}
