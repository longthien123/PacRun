using UnityEngine;
using UnityEngine.UI; // nếu dùng TMP, đổi thành TMPro
using TMPro;

public class MusicToggleButton : MonoBehaviour
{
    public Button button;      // gán button UI
    public TMP_Text buttonText; // gán TextMeshPro Text hiển thị trạng thái

    void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnToggleClicked);

        UpdateButtonText();
    }

    void OnToggleClicked()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMusic();
            UpdateButtonText();
        }
    }

    void UpdateButtonText()
    {
        if (buttonText != null && MusicManager.Instance != null)
        {
            buttonText.text = MusicManager.Instance.IsMusicOn() ? "ON" : "OFF";
        }
    }
}
