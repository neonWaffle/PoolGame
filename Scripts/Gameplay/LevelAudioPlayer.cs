using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAudioPlayer : MonoBehaviour
{
    [SerializeField] AudioClip levelBackgroundAudio;
    [SerializeField] AudioClip winAudio;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = levelBackgroundAudio;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Start()
    {
        MatchManager.Instance.OnMatchOver += PlayWinAudio;
    }

    void OnDestroy()
    {
        if (MatchManager.Instance != null)
            MatchManager.Instance.OnMatchOver += PlayWinAudio;
    }

    void PlayWinAudio(int winnerId)
    {
        audioSource.clip = winAudio;
        audioSource.loop = false;
        audioSource.Play();
    }
}
