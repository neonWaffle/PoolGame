using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    Cue cue;
    CueBall cueBall;
    bool canPickUpCueBall = false;
    bool cueBallPickedUp = false;
    public event Action OnCueBallPlacementEnabled;
    public event Action OnCueBallPickedUp;

    void Start()
    {
        cue = FindObjectOfType<Cue>();
        cueBall = FindObjectOfType<CueBall>();
        cueBall.OnPotted += EnableCueBallPlacement;
        MatchManager.Instance.OnTurnStarted += StartTurn;
        cue.OnShot += ShotMade;
        cue.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (MatchManager.Instance != null)
            MatchManager.Instance.OnTurnStarted -= StartTurn;
        if (cue != null)
            cue.OnShot -= ShotMade;
        if (cueBall != null)
            cueBall.OnPotted -= EnableCueBallPlacement;
    }

    void EnableCueBallPlacement()
    {
        canPickUpCueBall = true;
    }

    void PlaceCueBall()
    {
        cueBall.ResetPlacement();
        cueBall.gameObject.SetActive(true);
        cue.ResetAim();
        OnCueBallPlacementEnabled?.Invoke();
    }

    void ShotMade()
    {
        canPickUpCueBall = false;
        cue.gameObject.SetActive(false);
    }

    void EnableCue()
    {
        cue.gameObject.SetActive(true);
    }

    void StartTurn()
    {
        if (canPickUpCueBall)
            PlaceCueBall();
        EnableCue();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out RaycastHit hit);
            if (canPickUpCueBall && Input.touches[0].phase == TouchPhase.Began && hit.transform != null && hit.transform.gameObject == cueBall.gameObject)
            {
                cue.gameObject.SetActive(false);
                cueBallPickedUp = true;
                OnCueBallPickedUp?.Invoke();
            }
            if (cueBallPickedUp && Input.touches[0].phase == TouchPhase.Ended)
            {
                cue.gameObject.SetActive(true);
                cueBallPickedUp = false;
            }
            if (cueBallPickedUp && Input.touches[0].phase == TouchPhase.Moved)
            {
                cueBall.SetPosition(hit.point);
            }
            else if (cue.isActiveAndEnabled && cue.CanBeAimed)
            {
                cue.Aim(hit.point);
            }
        }
    }
}
