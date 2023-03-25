using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BallType { SOLID, STRIPES, CUE, BLACK }

class PlayerInfo
{
    public PlayerInfo(int playerId)
    {
        id = playerId;
        pottedBalls = 0;
        hasExtraTurn = false;
    }
    public int id;
    public BallType ballType;
    public int pottedBalls;
    public bool hasExtraTurn;
}

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }
    public event Action OnTurnStarted;
    [SerializeField] float checkFrequency = 0.5f;
    Ball[] balls;
    bool playerOneTurn = true;
    bool ballsAssigned = false;
    PlayerInfo playerOneInfo, playerTwoInfo;
    PlayerInfo currentPlayer, otherPlayer;
    [SerializeField] int ballsToPot = 7;
    public event Action<int, Ball> OnBallPotted;
    public event Action<int> OnCurrentPlayerAssigned;
    public event Action<int, float> OnMatchStarted;
    public event Action<int> OnMatchOver;
    [SerializeField] float delayTilStart = 3f;
    enum BallHitCheck { NO_HITS, WRONG_HIT, RIGHT_HIT }
    BallHitCheck ballHitStatus = BallHitCheck.NO_HITS;
    bool wrongBallPotted = false;
    bool blackBallPotted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerOneInfo = new PlayerInfo(1);
        playerTwoInfo = new PlayerInfo(2);
    }

    void Start()
    {
        balls = FindObjectsOfType<Ball>();
        var pockets = FindObjectsOfType<Pocket>();
        foreach (var pocket in pockets)
            pocket.OnBallPotted += BallPotted;
        var cueBall = FindObjectOfType<CueBall>();
        cueBall.OnBallHit += CheckIfRightBallHit;
        cueBall.OnShot += BallShot;
        StartCoroutine(StartMatch());
    }

    void OnDestroy()
    {
        var pockets = FindObjectsOfType<Pocket>();
        foreach (var pocket in pockets)
            pocket.OnBallPotted -= BallPotted;
        var cueBall = FindObjectOfType<CueBall>();
        if (cueBall != null)
        {
            cueBall.OnBallHit -= CheckIfRightBallHit;
            cueBall.OnShot -= BallShot;
        }
    }

    void CheckIfRightBallHit(BallType ballType)
    {
        //No need to make additional checks if the cue ball has hit any ball already
        if (ballHitStatus == BallHitCheck.WRONG_HIT || ballHitStatus == BallHitCheck.RIGHT_HIT)
            return;
        if (!ballsAssigned || ballType == currentPlayer.ballType || (ballType == BallType.BLACK && currentPlayer.pottedBalls == ballsToPot))
            ballHitStatus = BallHitCheck.RIGHT_HIT;
        else
            ballHitStatus = BallHitCheck.WRONG_HIT;
    }

    void BallPotted(Ball ball)
    {
        if (!ballsAssigned && ball.Type != BallType.CUE && ball.Type != BallType.BLACK)
        {
            ballsAssigned = true;
            currentPlayer.ballType = ball.Type;
            otherPlayer.ballType = ball.Type == BallType.SOLID ? BallType.STRIPES : BallType.SOLID;
        }
        switch (ball.Type)
        {
            case BallType.SOLID:
                if (currentPlayer.ballType == BallType.SOLID)
                {
                    OnBallPotted?.Invoke(currentPlayer.id, ball);
                    currentPlayer.pottedBalls++;
                    currentPlayer.hasExtraTurn = true;
                }
                else if (currentPlayer.ballType == BallType.STRIPES)
                {
                    OnBallPotted?.Invoke(otherPlayer.id, ball);
                    wrongBallPotted = true;
                    otherPlayer.pottedBalls++;
                }
                break;
            case BallType.STRIPES:
                if (currentPlayer.ballType == BallType.STRIPES)
                {
                    OnBallPotted?.Invoke(currentPlayer.id, ball);
                    currentPlayer.pottedBalls++;
                    currentPlayer.hasExtraTurn = true;
                }
                else if (currentPlayer.ballType == BallType.SOLID)
                {
                    OnBallPotted?.Invoke(otherPlayer.id, ball);
                    wrongBallPotted = true;
                    otherPlayer.pottedBalls++;
                }
                break;
            case BallType.CUE:
                wrongBallPotted = true;
                break;
            case BallType.BLACK:
                blackBallPotted = true;
                break;
        }
    }

    void MatchOver()
    {
        var winner = currentPlayer.pottedBalls < ballsToPot || wrongBallPotted ? otherPlayer : currentPlayer;
        OnMatchOver?.Invoke(winner.id);
    }

    IEnumerator StartMatch()
    {
        //Reset everything here
        playerOneInfo = new PlayerInfo(1);
        playerTwoInfo = new PlayerInfo(2);
        playerOneTurn = UnityEngine.Random.Range(1, 3) == 1;
        currentPlayer = playerOneTurn ? playerOneInfo : playerTwoInfo;
        otherPlayer = playerOneTurn ? playerTwoInfo : playerOneInfo;
        ballsAssigned = false;
        wrongBallPotted = false;
        blackBallPotted = false;
        foreach (var ball in balls)
        {
            ball.gameObject.SetActive(true);
            ball.ResetPlacement();
        }
        OnMatchStarted?.Invoke(playerOneTurn ? 1 : 2, delayTilStart);
        yield return new WaitForSeconds(delayTilStart); //Giving some time for UI to be displayed
        StartTurn();
    }

    void BallShot()
    {
        StartCoroutine(CheckIfBallsStopped());
    }

    IEnumerator CheckIfBallsStopped()
    {
        var checkWait = new WaitForSeconds(checkFrequency);
        yield return checkWait;
        while (true)
        {
            bool stoppedMoving = true;
            foreach (var ball in balls)
            {
                if (ball.isActiveAndEnabled && !ball.StoppedMoving())
                {
                    stoppedMoving = false;
                    break; //No point in checking anymore cause not all balls have stopped moving anyway
                }
            }
            if (stoppedMoving)
                break;
            yield return checkWait;
        }
        if (blackBallPotted)
            MatchOver();
        else
        {
            AssignCurrentPlayer();
            StartTurn();
        }
    }

    void AssignCurrentPlayer()
    {
        if (ballHitStatus == BallHitCheck.WRONG_HIT || ballHitStatus == BallHitCheck.NO_HITS || wrongBallPotted)
            currentPlayer.hasExtraTurn = false;
        if (!currentPlayer.hasExtraTurn)
        {
            playerOneTurn = !playerOneTurn;
            currentPlayer = playerOneTurn ? playerOneInfo : playerTwoInfo;
            otherPlayer = playerOneTurn ? playerTwoInfo : playerOneInfo;
        }
        currentPlayer.hasExtraTurn = false;
        if (ballHitStatus == BallHitCheck.WRONG_HIT || ballHitStatus == BallHitCheck.NO_HITS || wrongBallPotted)
            currentPlayer.hasExtraTurn = true;
        ballHitStatus = BallHitCheck.NO_HITS;
        OnCurrentPlayerAssigned?.Invoke(currentPlayer.id);
    }

    void StartTurn()
    {
        wrongBallPotted = false;
        OnTurnStarted?.Invoke();
    }
}
