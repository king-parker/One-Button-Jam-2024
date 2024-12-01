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

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {

    }

    public void OnUIAccept()
    {
        switch (gameState)
        {
            case GameState.TitleScreen:
                StartGame();
                break;
            case GameState.GameOver:
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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
