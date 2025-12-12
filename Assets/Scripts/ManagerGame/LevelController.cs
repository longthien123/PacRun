
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelController : MonoBehaviour
{
    // // Method xử lý khi bấm nút YES - chuyển qua level_3
    public void OnYesLevel2ButtonClicked()
    {
        SceneManager.LoadScene("ChristmasDay");
    }
    // Method xử lý khi bấm nút YES - chuyển qua level_3
    public void OnYesLevel3ButtonClicked()
    {
        SceneManager.LoadScene("Happy New Year");
    }

    // Method xử lý khi bấm nút NO - quay về Home
    public void OnNoButtonClicked()
    {
        SceneManager.LoadScene("HomeMenu");
    }
    public void OnYesOverButtonClicked()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.lastSceneName))
            SceneManager.LoadScene(GameManager.Instance.lastSceneName);
        else
            SceneManager.LoadScene("HomeMenu");
    }
}