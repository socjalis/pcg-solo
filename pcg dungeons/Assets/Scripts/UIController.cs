using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject pauseModal;
    bool modalState;

    public void onPauseButtonClicked()
    {
        Time.timeScale = 0;
        modalState = !modalState;
        pauseModal.SetActive(modalState);
    }

    public void onContinueClicked()
    {
        Time.timeScale = 1f;
        modalState = !modalState;
        pauseModal.SetActive(modalState);
    }

    public void onMainMenuClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
