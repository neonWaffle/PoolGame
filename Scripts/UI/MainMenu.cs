using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.Instance.StartGame();
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
