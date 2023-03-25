using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] MatchOverPanel matchOverPanel;
    [SerializeField] CanvasGroup startingPlayerPanel;
    [SerializeField] TextMeshProUGUI startingPlayerText;
    [SerializeField] float textAnimScale = 1.5f;
    [SerializeField] PlayerInfoPanel playerOnePanel;
    [SerializeField] PlayerInfoPanel playerTwoPanel;
    [SerializeField] CanvasGroup cueBallPlacementPanel;

    void Awake()
    {
        ResetUI();
    }

    void Start()
    {
        MatchManager.Instance.OnBallPotted += UpdatePottedBalls;
        MatchManager.Instance.OnMatchStarted += ShowMatchStartUI;
        MatchManager.Instance.OnCurrentPlayerAssigned += ShowTurnInfo;
        MatchManager.Instance.OnMatchOver += MatchOver;
        var playerController = FindObjectOfType<PlayerController>();
        playerController.OnCueBallPlacementEnabled += EnableCueBallPlacementUI;
        playerController.OnCueBallPickedUp += DisableCueBallPlacementUI;
        var cue = FindObjectOfType<Cue>();
        cue.OnShot += DisableCueBallPlacementUI;
    }

    void OnDestroy()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnBallPotted -= UpdatePottedBalls;
            MatchManager.Instance.OnMatchStarted -= ShowMatchStartUI;
            MatchManager.Instance.OnCurrentPlayerAssigned -= ShowTurnInfo;
            MatchManager.Instance.OnMatchOver -= MatchOver;
        }
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.OnCueBallPlacementEnabled -= EnableCueBallPlacementUI;
            playerController.OnCueBallPickedUp -= DisableCueBallPlacementUI;
        }
        var cue = FindObjectOfType<Cue>();
        if (cue != null)
            cue.OnShot -= DisableCueBallPlacementUI;
    }

    void EnableCueBallPlacementUI()
    {
        cueBallPlacementPanel.gameObject.SetActive(true);
    }

    void DisableCueBallPlacementUI()
    {
        cueBallPlacementPanel.transform.localScale = Vector3.one;
        cueBallPlacementPanel.gameObject.SetActive(false);
    }

    void ResetUI()
    {
        matchOverPanel.gameObject.SetActive(false);
        startingPlayerPanel.gameObject.SetActive(false);
        cueBallPlacementPanel.gameObject.SetActive(false);
        playerOnePanel.ResetPanel();
        playerTwoPanel.ResetPanel();
    }

    void ShowMatchStartUI(int startingPlayerId, float duration)
    {
        var playerController = FindObjectOfType<PlayerController>();
        playerController.OnCueBallPlacementEnabled += EnableCueBallPlacementUI;
        playerController.OnCueBallPickedUp += DisableCueBallPlacementUI;
        ResetUI();
        StartCoroutine(DisplayStartingPlayerPanel(startingPlayerId, duration));
        ShowTurnInfo(startingPlayerId);
    }

    IEnumerator DisplayStartingPlayerPanel(int startingPlayerId, float duration)
    {
        startingPlayerText.text = "Player " + startingPlayerId + " starts!";
        startingPlayerPanel.gameObject.SetActive(true);
        startingPlayerPanel.alpha = 0f;
        startingPlayerPanel.DOFade(1f, duration / 2f);
        startingPlayerPanel.transform.DOScale(textAnimScale, duration / 2f);
        var wait = new WaitForSeconds(duration / 2f);
        yield return wait;
        startingPlayerPanel.DOFade(0f, duration / 2f);
        yield return wait;
        startingPlayerPanel.gameObject.SetActive(false);
    }

    void UpdatePottedBalls(int playerId, Ball pottedBall)
    {
        if (pottedBall.Type == BallType.CUE)
            return;
        if (playerId == 1)
            playerOnePanel.SetBall(pottedBall.BallSprite);
        else
            playerTwoPanel.SetBall(pottedBall.BallSprite);
    }

    void MatchOver(int winnerId)
    {
        var playerController = FindObjectOfType<PlayerController>();
        playerController.OnCueBallPlacementEnabled -= EnableCueBallPlacementUI;
        playerController.OnCueBallPickedUp -= DisableCueBallPlacementUI;
        matchOverPanel.gameObject.SetActive(true);
        matchOverPanel.DisplayPanel(winnerId);
    }

    void ShowTurnInfo(int playerId)
    {
        if (playerId == 1)
        {
            playerTwoPanel.UnmarkAsCurrentPlayer();
            playerOnePanel.MarkAsCurrentPlayer();
        }
        else
        {
            playerTwoPanel.MarkAsCurrentPlayer();
            playerOnePanel.UnmarkAsCurrentPlayer();
        }
    }
}
