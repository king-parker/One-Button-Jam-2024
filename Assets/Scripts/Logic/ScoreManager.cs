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
        currentScore = GameObject.FindWithTag("Player").transform.position.x;
        SetScoreText();
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
