using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerSelector : MonoBehaviour, ISelector
{
    public PlayerController player;
    public Slider powerSlider;
    public float maxPower;
    public float minPower;
    public float speed;

    private bool isIncreasing = true;
    private bool indicatorRunning = false;

    void Start()
    {
        powerSlider.maxValue = maxPower;
        powerSlider.value = minPower;

        if (player.GetState() != PlayerController.PlayerState.PowerSelect) { HideIndicator(); }
    }

    void Update()
    {
        if (!indicatorRunning) { return; }

        if (powerSlider.maxValue != maxPower) { powerSlider.maxValue = maxPower; }
        
        float currentPower = powerSlider.value;
        float powerCheck;
        bool changeDirections = false;
        float powerChange;
        float delta = speed * Time.deltaTime;

        if (isIncreasing)
        {
            powerCheck = maxPower - currentPower;
        }
        else
        {
            powerCheck = currentPower - minPower;
        }

        if (powerCheck < delta)
        {
            powerChange = powerCheck;
            changeDirections = true;
        }
        else
        {
            powerChange = delta;
        }

        powerSlider.value += isIncreasing ? powerChange : -powerChange;

        if (changeDirections) { isIncreasing = !isIncreasing; }
    }

    [ContextMenu("Start Power Indicator")]
    public void StartIndicator()
    {
        powerSlider.maxValue = maxPower;
        powerSlider.value = minPower;
        indicatorRunning = true;
        this.gameObject.SetActive(true);
    }

    [ContextMenu("Stop Power Indicator")]
    public void StopIndicator()
    {
        player.SetPower(powerSlider.value);
        indicatorRunning = false;
    }

    [ContextMenu("Hide Power Indicator")]
    public void HideIndicator()
    {
        this.gameObject.SetActive(false);
    }
}
