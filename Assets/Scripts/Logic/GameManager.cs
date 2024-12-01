using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static bool firstLoad = true;

    public PlayerController player;
    public GameObject chunkContainer;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public UIControls controls;

    private GameState gameState;
    private bool showTitleScreen;

    public enum GameState
    {
        TitleScreen,
        Gameplay,
        GameOver
    }

    private void Awake()
    {
        if (firstLoad)
        {
            showTitleScreen = true;
            firstLoad = false;
            gameState = GameState.TitleScreen;
        }
        else
        {
            showTitleScreen = false;
        }

        controls = new UIControls();
        controls.UI.UIAccept.canceled += _ => OnUIAccept();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        if (!showTitleScreen)
        {
            // Delay call to start game so player controls work
            titleScreen.SetActive(false);
            Invoke(nameof(StartWithGameplay), 0.25f);
        }
    }

    public void StartWithGameplay()
    {
        StartGame();
    }

    public void OnUIAccept()
    {
        switch (gameState)
        {
            case GameState.TitleScreen:
                StartGame();
                break;
            case GameState.GameOver:
                RestartGame();
                break;
            default:
                break;
        }
    }

    public void StartGame()
    {
        titleScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        gameState = GameState.Gameplay;
        player.SetState(PlayerController.PlayerState.AngleSelect);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        gameState = GameState.GameOver;
        player.SetState(PlayerController.PlayerState.Disabled);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
