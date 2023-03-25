using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CueBall : Ball
{
    public event Action<BallType> OnBallHit;
    public event Action OnShot;
    public event Action OnPotted;
    Transform minExtents;
    Transform maxExtents;

    void Start()
    {
        minExtents = GameObject.FindGameObjectWithTag("MinExtents").transform;
        maxExtents = GameObject.FindGameObjectWithTag("MaxExtents").transform;
    }

    void OnEnable()
    {
        ResetPlacement();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        var ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
            OnBallHit?.Invoke(ball.Type);
    }

    public void Shoot(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
        OnShot?.Invoke();
    }

    public void SetPosition(Vector3 pos)
    {
        var newPos = pos;
        newPos.x = Mathf.Clamp(newPos.x, minExtents.transform.position.x + Radius, maxExtents.transform.position.x - Radius);
        newPos.y = 0;
        newPos.z = Mathf.Clamp(newPos.z, minExtents.transform.position.z + Radius, maxExtents.transform.position.z - Radius);
        transform.position = newPos;
    }

    protected override IEnumerator Disable()
    {
        OnPotted?.Invoke();
        yield return new WaitForSeconds(timeTilDisable);
        gameObject.SetActive(false);
    }
}
