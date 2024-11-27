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

    //void Awake()
    //{
    //    StopIndicator();
    //    HideIndicator();
    //}

    // Start is called before the first frame update
    void Start()
    {
        powerSlider.maxValue = maxPower;
        powerSlider.value = minPower;

        if (player.GetState() != PlayerController.PlayerState.PowerSelect) { HideIndicator(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (!indicatorRunning) { return; }

        if (powerSlider.maxValue != maxPower) { powerSlider.maxValue = maxPower; }
        
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
        //print("Starting power indicator");
        powerSlider.maxValue = maxPower;
        powerSlider.value = minPower;
        indicatorRunning = true;
        this.gameObject.SetActive(true);
    }

    [ContextMenu("Stop Power Indicator")]
    public void StopIndicator()
    {
        //print("Stopping power indicator");
        player.SetPower(powerSlider.value);
        indicatorRunning = false;
    }

    [ContextMenu("Hide Power Indicator")]
    public void HideIndicator()
    {
        //print("Hiding power indicator");
        this.gameObject.SetActive(false);
    }
}
