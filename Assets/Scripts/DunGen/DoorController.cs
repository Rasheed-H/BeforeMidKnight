using UnityEngine;

/// <summary>
/// Controls the behavior of a door in the game, including its locked and unlocked state.
/// </summary>
public class DoorController : MonoBehaviour
{
    public Sprite openDoorSprite;
    public Sprite closedDoorSprite;

    private SpriteRenderer spriteRenderer;
    private Collider2D blockingCollider;
    private bool isUnlocked = true;


    /// <summary>
    /// Called when the DoorController script is initialized.
    /// Gets references to the SpriteRenderer and Collider2D components attached to the door.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        blockingCollider = GetComponent<Collider2D>();
    }


    /// <summary>
    /// Called when the scene starts.
    /// Sets the door's initial state (either open or closed) based on whether the door is unlocked.
    /// </summary>
    void Start()
    {
        if (!gameObject.activeInHierarchy)
            return;

        SetDoorState(isUnlocked);
    }


    /// <summary>
    /// Sets the state of the door to either unlocked (open) or locked (closed),
    /// updating the door's sprite and enabling or disabling the blocking collider.
    /// </summary>
    /// <param name="unlocked">A boolean that determines whether the door should be unlocked (true) or locked (false).</param>
    public void SetDoorState(bool unlocked)
    {

        if (!gameObject.activeInHierarchy)
            return;
        
        isUnlocked = unlocked;

        if (isUnlocked)
        {
            spriteRenderer.sprite = openDoorSprite;
            blockingCollider.enabled = false; 
        }
        else
        {
            spriteRenderer.sprite = closedDoorSprite;
            blockingCollider.enabled = true; 
        }
    }
}
