using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MatchOverPanel : MonoBehaviour
{
    CanvasGroup panel;
    [SerializeField] TextMeshProUGUI winnerText;
    [SerializeField] float pauseDuration = 0.1f;
    [SerializeField] float animDuration = 0.5f;
    [SerializeField] float textAnimScale = 1.05f;
    [SerializeField] float buttonAnimScaleMin = 0.95f;
    [SerializeField] float buttonAnimScaleMax = 1.05f;
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button quitButton;

    void Awake()
    {
        panel = GetComponentInChildren<CanvasGroup>();
    }

    public void DisplayPanel(int winnerId)
    {
        restartButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        restartButton.transform.localScale = Vector3.one * buttonAnimScaleMin;
        mainMenuButton.transform.localScale = Vector3.one * buttonAnimScaleMin;
        quitButton.transform.localScale = Vector3.one * buttonAnimScaleMin;
        winnerText.text = "Player " + winnerId.ToString() + " won";
        panel.alpha = 0f;
        var anim = DOTween.Sequence();
        anim.Append(panel.transform.DOScale(textAnimScale, animDuration))
            .Join(panel.DOFade(1f, animDuration))
            .Append(panel.transform.DOScale(Vector3.one, animDuration))
            .AppendInterval(pauseDuration)
            .AppendCallback(() => restartButton.gameObject.SetActive(true))
            .Append(restartButton.transform.DOScale(Vector3.one * buttonAnimScaleMax, animDuration))
            .AppendCallback(() => mainMenuButton.gameObject.SetActive(true))
            .Join(restartButton.transform.DOScale(Vector3.one, animDuration))
            .Join(mainMenuButton.transform.DOScale(Vector3.one * buttonAnimScaleMax, animDuration))
            .AppendCallback(() => quitButton.gameObject.SetActive(true))
            .Join(mainMenuButton.transform.DOScale(Vector3.one, animDuration))
            .Join(quitButton.transform.DOScale(Vector3.one * buttonAnimScaleMax, animDuration))
            .Append(quitButton.transform.DOScale(Vector3.one, animDuration));
    }

    public void Restart()
    {
        SceneLoader.Instance.ReloadLevel();
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }

    public void ReturnToMainMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }
}
