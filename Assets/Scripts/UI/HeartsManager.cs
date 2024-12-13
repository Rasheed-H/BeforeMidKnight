using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the display of hearts in the UI based on the player's health and max health.
/// </summary>
public class HeartsManager : MonoBehaviour
{
    public GameObject heartPrefab; 
    private List<HeartController> hearts = new List<HeartController>();

    /// <summary>
    /// Initializes the hearts based on the player's max health.
    /// </summary>
    public void InitializeHearts(int maxHealth)
    {
        ClearHearts();

        int totalHearts = Mathf.CeilToInt(maxHealth / 2f);
        for (int i = 0; i < totalHearts; i++)
        {
            CreateEmptyHeart();
        }
    }

    /// <summary>
    /// Updates the hearts to reflect the player's current health.
    /// </summary>
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartState = Mathf.Clamp(currentHealth - (i * 2), 0, 2); 
            hearts[i].SetHeartImage((HeartStatus)heartState);
        }
    }

    /// <summary>
    /// Creates and adds an empty heart to the UI.
    /// </summary>
    private void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab, transform);
        HeartController heartController = newHeart.GetComponent<HeartController>();
        heartController.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartController);
    }

    /// <summary>
    /// Clears all heart UI elements.
    /// </summary>
    private void ClearHearts()
    {
        foreach (Transform heart in transform)
        {
            Destroy(heart.gameObject);
        }
        hearts.Clear();
    }
}
