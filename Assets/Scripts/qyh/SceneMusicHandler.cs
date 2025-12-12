using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicHandler : MonoBehaviour
{
    void Start()
    {
        if (MusicManager.Instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            MusicManager.Instance.PlayMusicForScene(sceneName);
        }
    }
}
