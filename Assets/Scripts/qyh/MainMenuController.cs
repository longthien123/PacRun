// Scripts/MainMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject instructionsPanel;

    // ====== THÊM PAGE VÀO ĐÂY ======
    public GameObject pageGeneral;
    public GameObject pageLevel;
    public GameObject pageEntity;
    public GameObject pageGhost;

    public void PlayGame()
    {
        //SceneManager.LoadScene("Gameplay");
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

//     private IEnumerator QuitRoutine()
//     {
//         // Load ExitMenu additive để giữ Coroutine chạy
//         SceneManager.LoadScene("ExitMenu", LoadSceneMode.Additive);

//         // Chờ 1 giây để người chơi nhìn thấy ExitMenu
//         yield return new WaitForSeconds(1f);

//         // Thoát game
//         Application.Quit();

// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #endif
//     }

    // =====================================================
    // ==============  CÁC PAGE SWITCH FUNCTIONS ===========
    // =====================================================

    public void ShowGeneralPage()
    {
        pageGeneral.SetActive(true);
        pageLevel.SetActive(false);
        pageEntity.SetActive(false);
        pageGhost.SetActive(false);
    }

    public void ShowLevelPage()
    {
        pageGeneral.SetActive(false);
        pageLevel.SetActive(true);
        pageEntity.SetActive(false);
        pageGhost.SetActive(false);
    }

    public void ShowEntityPage()
    {
        pageGeneral.SetActive(false);
        pageLevel.SetActive(false);
        pageEntity.SetActive(true);
        pageGhost.SetActive(false);
    }

    public void ShowGhostPage()
    {
        pageGeneral.SetActive(false);
        pageLevel.SetActive(false);
        pageEntity.SetActive(false);
        pageGhost.SetActive(true);
    }

}
