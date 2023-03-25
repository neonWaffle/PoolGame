using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Cue : MonoBehaviour
{
    [Header("Cue controls")]
    [SerializeField] float offset = 0.25f;
    [SerializeField] float maxShotSpeed = 20f;
    [SerializeField] float maxShotStrength = 10f;
    [SerializeField] float maxMoveDistance = 3f;
    public event Action OnShot;
    public bool CanBeAimed => !settingStrength && !isShooting;
    bool settingStrength = false;
    bool isShooting = false;
    float shotStrength = 0f;
    Rigidbody rb;
    Slider strengthSlider;
    CueBall cueBall;
    [Header("Trajectory visualisation")]
    [SerializeField] GameObject visualiserBall;
    [SerializeField] LineRenderer cueTrajectory;
    [SerializeField] LineRenderer hitBallTrajectory;
    [SerializeField] float visualiserLength = 1.5f;

    void Awake()
    {
        cueBall = FindObjectOfType<CueBall>();
        offset += cueBall.Radius;
        rb = GetComponent<Rigidbody>();
        GetComponent<Collider>().isTrigger = true;
        strengthSlider = GetComponentInChildren<Slider>();
        strengthSlider.maxValue = maxShotStrength;
        strengthSlider.value = strengthSlider.minValue;
        visualiserBall = Instantiate(visualiserBall);
        transform.position = cueBall.transform.position - transform.forward * offset;
    }

    void Start()
    {
        isShooting = false;
        settingStrength = false;
    }

    void OnDisable()
    {
        if (visualiserBall != null)
            visualiserBall.SetActive(false);
    }

    void OnEnable()
    {
        visualiserBall.SetActive(true);
        ResetAim();
    }

    //Makes the cue aim forwards and sets the position with an offset from the cue ball
    public void ResetAim()
    {
        Physics.SphereCast(transform.position, cueBall.Radius, transform.forward, out RaycastHit hit, Mathf.Infinity);
        var dir = (hit.point - transform.position);
        dir.y = 0f;
        dir.Normalize();
        transform.position = cueBall.transform.position - dir * offset;
        transform.LookAt(cueBall.transform.position);
        VisualiseTrajectory();
    }

    public void Aim(Vector3 pos)
    {
        var dir = (pos - transform.position);
        dir.y = 0f;
        dir.Normalize();
        transform.position = cueBall.transform.position - dir * offset;
        transform.LookAt(cueBall.transform.position);
        VisualiseTrajectory();
    }

    void VisualiseTrajectory()
    {
        Physics.SphereCast(cueBall.transform.position, cueBall.Radius, transform.forward, out RaycastHit hit);
        cueTrajectory.SetPosition(0, cueBall.transform.position);
        cueTrajectory.SetPosition(1, hit.point + hit.normal * cueBall.Radius);
        visualiserBall.transform.position = hit.point + hit.normal * cueBall.Radius;
        if (hit.transform.GetComponent<Ball>() != null)
        {
            hitBallTrajectory.gameObject.SetActive(true);
            hitBallTrajectory.SetPosition(0, hit.transform.position + hit.normal * cueBall.Radius);
            hitBallTrajectory.SetPosition(1, hit.transform.position - hit.normal * visualiserLength);
        }
        else
        {
            hitBallTrajectory.gameObject.SetActive(false);
        }
    }

    //UI
    public void SetShotStrength(float strength)
    {
        if (isShooting)
            return;
        settingStrength = true;
        shotStrength = strength;
        var dir = cueBall.transform.position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        transform.position = cueBall.transform.position - dir * offset - dir * (strength / maxShotStrength * maxMoveDistance);
    }

    //UI
    public void Shoot()
    {
        settingStrength = false;
        //Player should have an option to release without shooting if they want to change the direction of the cue
        if (shotStrength > 0f)
            StartCoroutine(MoveTowardsBall());
    }

    IEnumerator MoveTowardsBall()
    {
        isShooting = true;
        visualiserBall.SetActive(false);
        var targetPos = cueBall.transform.position;
        while (Vector3.Distance(rb.position, targetPos) > 0.001f)
        {
            rb.MovePosition(Vector3.MoveTowards(rb.position, targetPos, shotStrength / maxShotStrength * maxShotSpeed * Time.deltaTime));
            yield return null;
        }
        cueBall.Shoot(transform.forward * shotStrength);
        strengthSlider.value = strengthSlider.minValue;
        isShooting = false;
        OnShot?.Invoke();
    }
}
