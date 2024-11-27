using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayer
{
    public AngleSelector angleSelector;
    public PowerSelector powerSelector;
    public PlayerControls controls;
    public Rigidbody2D rb;

    private float angle = 0;
    private float power = 0;
    private PlayerState state;
    private bool movedLastUpdate = false;

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

    void Start()
    {
        state = PlayerState.Disabled;

        // TODO: Remove when a start menu is implemented
        NextState();
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Disabled:
                return;
            case PlayerState.Jumping:
                if (rb.velocity.magnitude > 0)
                {
                    movedLastUpdate = true;
                }
                else if (rb.velocity.magnitude == 0 && !movedLastUpdate)
                {
                    NextState();
                }
                else
                {
                    movedLastUpdate = false;
                }
                break;
        }
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
        //print("Player launch angle set: " + angleValue);
        angle = angleValue;
    }

    public void SetPower(float powerValue)
    {
        //print("Player launch power set: " + powerValue);
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
        //print("Switching state\nFrom " + state + " to " + nextState);
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
                rb.AddForce(new Vector2(power * Mathf.Cos(Mathf.Deg2Rad * angle), power * Mathf.Sin(Mathf.Deg2Rad * angle)));
                movedLastUpdate = true;
                break;
        }

        state = nextState;
        // TODO: Get rid of when jumping is implemented
        //if (state == PlayerState.Jumping) { NextState(); }
    }

    public PlayerState GetState() { return state; }
}
