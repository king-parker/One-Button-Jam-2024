using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Text Fields")]
    public Text scoreText;
    public Text statsText;

    private void OnEnable()
    {
        ScoreManager.OnFinalScore += SetScore;
    }

    private void OnDisable()
    {
        ScoreManager.OnFinalScore -= SetScore;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetScore(float score)
    {
        scoreText.text = $"Score: {score:F0}";
    }
}
