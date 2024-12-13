using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;

    /// <summary>
    /// Plays the assigned scene-specific music when the scene starts.
    /// </summary>
    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(sceneMusic);
        }
    }
}
