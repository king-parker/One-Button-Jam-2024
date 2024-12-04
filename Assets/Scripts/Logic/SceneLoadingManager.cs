using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.OnBoot += GameBoot;
        GameManager.OnGameStarted += UnloadTitle;
        GameManager.OnRestart += ReloadGameScene;
    }

    void OnDisable()
    {
        GameManager.OnBoot -= GameBoot;
        GameManager.OnGameStarted -= UnloadTitle;
        GameManager.OnRestart -= ReloadGameScene;
    }

    public void GameBoot()
    {
        SceneManager.LoadScene(GameManager.GAME_SCENE, LoadSceneMode.Additive);
        SceneManager.LoadScene(GameManager.TITLE_SCENE, LoadSceneMode.Additive);
        SceneManager.LoadScene(GameManager.GAME_OVER_SCENE, LoadSceneMode.Additive);
    }

    public void UnloadTitle()
    {
        var titleScene = SceneManager.GetSceneByName(GameManager.TITLE_SCENE);
        if (titleScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(titleScene);
        }
    }

    public Scene GetGameOverScene()
    {
        return SceneManager.GetSceneByName(GameManager.GAME_OVER_SCENE);
    }

    public void ReloadGameScene()
    {
        SceneManager.UnloadSceneAsync(GameManager.GAME_SCENE);
        SceneManager.LoadScene(GameManager.GAME_SCENE, LoadSceneMode.Additive);
    }
}
