using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScalingUIElement : MonoBehaviour
{
    [SerializeField] bool isLooped = false;
    [SerializeField] float animScale = 1.05f;
    Vector3 initialScale;
    [SerializeField] float animDuration = 0.5f;
    [SerializeField] float pauseDuration = 0.1f;

    void Awake()
    {
        initialScale = transform.localScale;
    }

    void OnEnable()
    {
        Animate();
    }

    void OnDisable()
    {
        transform.localScale = initialScale;
    }

    void Animate()
    {
        var anim = DOTween.Sequence();
        anim.SetLoops(isLooped ? -1 : 1, LoopType.Restart)
            .Append(transform.DOScale(animScale, animDuration))
            .AppendInterval(pauseDuration)
            .Append(transform.DOScale(initialScale, animDuration))
            .AppendInterval(pauseDuration);
    }
}
