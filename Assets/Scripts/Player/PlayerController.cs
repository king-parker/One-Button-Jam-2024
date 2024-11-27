using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayer
{
    public AngleSelector angleSelector;
    public PowerSelector powerSelector;
    public PlayerControls controls;

    private float angle = 0;
    private float power = 0;
    private PlayerState state;

    public enum PlayerState
    {
        Disabled,
        AngleSelect,
        PowerSelect,
        Jumping
    }

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Button.started += _ => ButtonPress();
        controls.Player.Button.canceled += _ => ButtonRelease();
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
        state = PlayerState.Disabled;

        // TODO: Remove when a start menu is implemented
        NextState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonPress()
    {
        //print("Button Pressed\nState: " + state);
        if (state == PlayerState.AngleSelect)
        {
            NextState();
        }
    }

    public void ButtonRelease()
    {
        //print("Button Released\nState: " + state);
        if (state == PlayerState.PowerSelect)
        {
            NextState();
        }
    }

    public void SetAngle(float angleValue)
    {
        angle = angleValue;
    }

    public void SetPower(float powerValue)
    {
        power = powerValue;
    }

    public void NextState()
    {
        //print("Auto next state transition");
        switch (state)
        {
            case PlayerState.Disabled:
                SetState(PlayerState.AngleSelect);
                break;
            case PlayerState.AngleSelect:
                SetState(PlayerState.PowerSelect);
                break;
            case PlayerState.PowerSelect:
                SetState(PlayerState.Jumping);
                break;
            case PlayerState.Jumping:
                SetState(PlayerState.AngleSelect);
                break;
        }
    }

    public void SetState(PlayerState nextState)
    {
        //print("Switching state\nCurrent state: " + state + "\nNext state: " +  nextState);
        switch (nextState)
        {
            case PlayerState.Disabled:
                break;
            case PlayerState.AngleSelect:
                angleSelector.StartIndicator();
                break;
            case PlayerState.PowerSelect:
                angleSelector.StopIndicator();
                powerSelector.StartIndicator();
                break;
            case PlayerState.Jumping:
                angleSelector.HideIndicator();
                powerSelector.StopIndicator();
                powerSelector.HideIndicator();
                break;
        }

        state = nextState;
        // TODO: Get rid of when jumping is implemented
        if (state == PlayerState.Jumping) { NextState(); }
    }
}
