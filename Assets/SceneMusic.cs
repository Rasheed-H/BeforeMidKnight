using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(sceneMusic);
        }
    }
}
