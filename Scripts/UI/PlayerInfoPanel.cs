using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    [SerializeField] Image[] ballImages;
    [SerializeField] Sprite emptyBallSprite;
    [SerializeField] Image turnIndicatorImage;
    [SerializeField] Sprite currentTurnSprite;
    [SerializeField] Sprite notCurrentTurnSprite;
    int currentBallId = 0;

    public void ResetPanel()
    {
        foreach (var image in ballImages)
            image.sprite = emptyBallSprite;
    }

    public void SetBall(Sprite sprite)
    {
        ballImages[currentBallId].sprite = sprite;
        currentBallId++;
    }

    public void MarkAsCurrentPlayer()
    {
        turnIndicatorImage.sprite = currentTurnSprite;
    }

    public void UnmarkAsCurrentPlayer()
    {
        turnIndicatorImage.sprite = notCurrentTurnSprite;
    }
}
