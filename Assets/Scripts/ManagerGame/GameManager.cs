using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string lastSceneName; // scene gameplay gần nhất (để retry)

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "game_over") return;
        lastSceneName = scene.name;
    }

    public void GameOver()
    {
        var current = SceneManager.GetActiveScene().name;
        if (current != "game_over")
            lastSceneName = current;
        SceneManager.LoadScene("game_over");
    }
}
