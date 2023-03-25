using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pocket : MonoBehaviour
{
    public event Action<Ball> OnBallPotted;

    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            ball.GetPotted();
            OnBallPotted?.Invoke(ball);
        }
    }
}
