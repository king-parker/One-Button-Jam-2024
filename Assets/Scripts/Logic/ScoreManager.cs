using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Calculation Weights")]
    public float distanceWeight = 10f;
    public float timeFactorLogBase = Mathf.Exp(1);
    public float minTimeFactor = 0.1f;
    public float timeFactorBufferTime = 1f;
    public float jumpPenaltyWeight = 20f;
    public float minScoreAddition = 0f;

    [Header("Score Text Reference")]
    public Text scoreText;

    public static event Action<float> OnFinalScore;

    private readonly string baseText = "Score: ";
    private float currentScore = 0f;

    private void OnEnable()
    {
        GameManager.OnGameStarted += SetScoreForGameStart;
        GameManager.OnGameOver += HideScore;
        GameManager.OnGameOver += SendFinalScore;
        PlayerController.OnJumpCompleted += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= SetScoreForGameStart;
        GameManager.OnGameOver -= HideScore;
        GameManager.OnGameOver -= SendFinalScore;
        PlayerController.OnJumpCompleted -= UpdateScore;
    }

    void Start()
    {
        if (GameManager.IsFirstGameStart())
        {
            HideScore();
        }
        else
        {
            SetScoreForGameStart();
        }
    }

    private void UpdateScore()
    {
        float timeEfficiencyFactor = CalcTimeFactor(PlayerData.DeltaSelectionTime);
        currentScore = Mathf.Max(minScoreAddition, ((PlayerData.PlayerDistance * distanceWeight * timeEfficiencyFactor) - jumpPenaltyWeight));
        SetScoreText();
    }

    private float CalcTimeFactor(float time)
    {
        // Use a time buffer to filter out spamming jump being favored by "quick decisions"
        // Time factor is 1 and doesn't drop until after the buffer time
        float adjustedTime = Mathf.Max(time - timeFactorBufferTime, 0);

        float calcualatedFactor = 1 / (1 + Mathf.Log((1 + time), timeFactorLogBase));

        // Use max so the time factor is capped at a minimum value
        return Mathf.Max(minTimeFactor, calcualatedFactor); 
    }

    private void SetScoreForGameStart()
    {
        currentScore = 0;
        SetScoreText();
        scoreText.gameObject.SetActive(true);
    }

    private void HideScore()
    {
        scoreText.gameObject.SetActive(false);
    }

    private void SetScoreText()
    {
        scoreText.text = $"{baseText}{currentScore:F0}";
    }

    private void SendFinalScore()
    {
        OnFinalScore?.Invoke(currentScore);
    }
}
