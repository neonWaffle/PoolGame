using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    protected Rigidbody rb;
    [SerializeField] float stopThreshold = 0.001f;
    [SerializeField] float hitAudioThreshold = 1f;
    [SerializeField] BallType type;
    public BallType Type => type;
    [SerializeField] Sprite ballSprite;
    public Sprite BallSprite => ballSprite;
    public float Radius { get; protected set; }
    public SphereCollider Collider { get; protected set; }
    Vector3 initialPos;
    [SerializeField] protected float timeTilDisable = 1f;
    public float TimeTilDisable => timeTilDisable;
    [SerializeField] AudioClip[] collisionClips;
    [SerializeField] AudioClip pottedClip;
    AudioSource audioSource;
    [SerializeField] float maxVolumeVelocity = 20f;

    void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        Radius = Collider.radius;
        initialPos = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    public void ReenableBall()
    {
        DeactivateGravity();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public bool StoppedMoving()
    {
        return rb.velocity.sqrMagnitude <= stopThreshold;
    }

    public void ResetPlacement()
    {
        transform.position = initialPos;
        ReenableBall();
    }

    public void ActivateGravity()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    public void DeactivateGravity()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void GetPotted()
    {
        audioSource.PlayOneShot(pottedClip);
        StartCoroutine(Disable());
    }

    protected virtual IEnumerator Disable()
    {
        yield return new WaitForSeconds(timeTilDisable);
        gameObject.SetActive(false);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>();
        if (collision.relativeVelocity.magnitude > hitAudioThreshold && ball == null
            || (ball != null && GetInstanceID() < collision.gameObject.GetInstanceID())) //makes sure that only one ball makes a sound during collision
            PlayCollisionAudio(collision.relativeVelocity.magnitude);
    }

    void PlayCollisionAudio(float velocity)
    {
        int clipId = Random.Range(1, collisionClips.Length);
        var currentClip = collisionClips[clipId];
        collisionClips[clipId] = collisionClips[0];
        collisionClips[0] = currentClip;
        audioSource.PlayOneShot(currentClip, velocity / maxVolumeVelocity);
    }
}
