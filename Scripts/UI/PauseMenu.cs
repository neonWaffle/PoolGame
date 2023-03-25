using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup pausePanel;
    [SerializeField] float fadeTime = 0.5f;

    void Awake()
    {
        pausePanel.gameObject.SetActive(false);
    }

    public void Pause()
    {
        pausePanel.gameObject.SetActive(true);
        pausePanel.alpha = 0f;
        pausePanel.DOFade(1f, fadeTime).OnComplete(() => Time.timeScale = 0f);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pausePanel.alpha = 1f;
        pausePanel.DOFade(0f, fadeTime).OnComplete(() => pausePanel.gameObject.SetActive(false));
    }

    public void Restart()
    {
        SceneLoader.Instance.ReloadLevel();
    }

    public void GoToMainMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
