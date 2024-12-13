using UnityEngine;

/// <summary>
/// Controls the state of a door in the dungeon. 
/// A door can be open, closed, or non-existent (no door).
/// </summary>
public class DoorController : MonoBehaviour
{
    public GameObject doorOpen;   
    public GameObject doorClosed; 
    public GameObject noDoor;     

    /// <summary>
    /// Sets the door state to open, closed, or no door.
    /// </summary>
    /// <param name="state">The state to set: "open", "closed", or "none".</param>
    public void SetDoorState(string state)
    {
        if (noDoor.activeSelf)
        {
            return;
        }

        doorOpen.SetActive(false);
        doorClosed.SetActive(false);
        noDoor.SetActive(false);

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
                noDoor.SetActive(true);
                break;
        }
    }
}
