using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Container")]
    public GameObject gameOverScreen;

    [Header("Text Fields")]
    public Text scoreText;
    public Text jumpStatText;
    public Text timeStatText;
    public Text distanceStatText;

    public static event Action<GameObject> OnGameOverScreenReady;

    private void OnEnable()
    {
        ScoreManager.OnFinalScore += SetResults;
    }

    private void OnDisable()
    {
        ScoreManager.OnFinalScore -= SetResults;
    }

    void Start()
    {
        OnGameOverScreenReady?.Invoke(gameOverScreen);
    }

    private void SetResults(float score)
    {
        int jumps = PlayerData.PlayerJumps;
        float time = PlayerData.SessionSelectionTime;
        float distance = PlayerData.PlayerDistance;

        scoreText.text = $"Score: {score:F0}";
        jumpStatText.text = $"Successful Jumps: {jumps}";
        timeStatText.text = $"Selection Time: {time:f2}s";
        distanceStatText.text = $"Distance Traveled: {distance:f0}";
    }
}
