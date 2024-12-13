using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public MusicManager musicManager;
    public SceneLoadingManager sceneLoadingManager;
    public UIControls uiControls;

    public const string TITLE_SCENE = "Title Scene";
    public const string GAME_SCENE = "Game Scene";
    public const string GAME_OVER_SCENE = "Game Over Scene";

    public static event Action OnBoot;
    public static event Action OnGameStarted;
    public static event Action OnGameOver;
    public static event Action OnRestart;

    private static bool isFirstGameStart = true;

    private GameState gameState;
    private GameObject gameOverScreen;

    public enum GameState
    {
        TitleScreen,
        Gameplay,
        GameOver
    }

    private void Awake()
    {
        uiControls = new UIControls();
        uiControls.UI.UIAccept.canceled += _ => OnUIAccept();
    }

    void OnEnable()
    {
        uiControls.Enable();
        GameOverManager.OnGameOverScreenReady += SetGameOverScreen;
    }

    void OnDisable()
    {
        uiControls.Disable();
        GameOverManager.OnGameOverScreenReady -= SetGameOverScreen;
    }

    void Start()
    {
        gameState = GameState.TitleScreen;
        OnBoot?.Invoke();
    }

    public static bool IsFirstGameStart()
    {
        return isFirstGameStart;
    }

    public void StartGame()
    {
        PlayerData.ResetData();
        gameState = GameState.Gameplay;
        OnGameStarted?.Invoke();
        isFirstGameStart = false;
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        OnGameOver?.Invoke();
        gameOverScreen?.SetActive(true);
    }

    public void Restart()
    {
        PlayerData.ResetData();
        gameState = GameState.Gameplay;
        gameOverScreen.SetActive(false);
        OnRestart?.Invoke();
    }

    private void OnUIAccept()
    {
        switch (gameState)
        {
            case GameState.TitleScreen:
                StartGame();
                break;
            case GameState.GameOver:
                Restart();
                break;
            default:
                break;
        }
    }

    private void SetGameOverScreen(GameObject gameOverScreen)
    {
        this.gameOverScreen = gameOverScreen;
    }
}
