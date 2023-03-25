using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    [SerializeField] Image loadingBar;
    Canvas canvas;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        canvas = GetComponentInChildren<Canvas>();
        DisableLoadingScreen();
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadLevelAsync("MainMenu"));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void StartGame()
    {
        StartCoroutine(LoadLevelAsync("Level"));
    }

    public void ReloadLevel()
    {
        StartCoroutine(LoadLevelAsync(SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadLevelAsync(string sceneTitle)
    {
        DisplayLoadingScreen();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTitle);
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingBar.fillAmount = progress;
            yield return null;
        }
        DisableLoadingScreen();
        Time.timeScale = 1f;
    }

    void DisplayLoadingScreen()
    {
        canvas.gameObject.SetActive(true);
    }

    void DisableLoadingScreen()
    {
        canvas.gameObject.SetActive(false);
    }
}
