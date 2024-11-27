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

    // Start is called before the first frame update
    void Start()
    {
        StopIndicator();
        HideIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        if (!indicatorRunning) { return; }

        float currentPower = powerSlider.value;
        float powerCheck;
        bool changeDirections = false;
        float powerChange;

        if (isIncreasing)
        {
            powerCheck = maxPower - currentPower;
        }
        else
        {
            powerCheck = currentPower - minPower;
        }

        if (powerCheck < speed)
        {
            powerChange = powerCheck;
            changeDirections = true;
        }
        else
        {
            powerChange = speed;
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
