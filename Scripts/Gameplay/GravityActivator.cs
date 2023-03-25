using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityActivator : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<Ball>();
        if (ball != null)
            ball.ActivateGravity();
    }

    void OnTriggerExit(Collider other)
    {
        var ball = other.GetComponent<Ball>();
        if (ball != null)
            ball.DeactivateGravity();
    }
}
