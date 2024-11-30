using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject doorOpen;    // Reference to the DoorOpen state GameObject
    public GameObject doorClosed;  // Reference to the DoorClosed state GameObject
    public GameObject noDoor;      // Reference to the NoDoor state GameObject

    /// <summary>
    /// Sets the door state to open, closed, or no door.
    /// </summary>
    /// <param name="state">The state to set: "open", "closed", or "none".</param>
    public void SetDoorState(string state)
    {
        // If NoDoor is active, don't allow state changes
        if (noDoor.activeSelf)
        {
            Debug.Log($"Door {gameObject.name} is set to NoDoor and cannot be changed.");
            return;
        }

        // Deactivate all states first
        doorOpen.SetActive(false);
        doorClosed.SetActive(false);
        noDoor.SetActive(false);

        // Activate the correct state
        switch (state.ToLower())
        {
            case "open":
                doorOpen.SetActive(true);
                break;

            case "closed":
                doorClosed.SetActive(true);
                break;

            case "none":
                noDoor.SetActive(true);
                break;

            default:
                Debug.LogWarning($"Invalid door state '{state}' for {gameObject.name}. Setting to NoDoor.");
                noDoor.SetActive(true);
                break;
        }
    }
}
