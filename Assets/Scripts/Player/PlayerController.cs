using System;
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

    public static event Action OnJumpCompleted;

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
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.Player.Button.started += _ => ButtonPress();
        controls.Player.Button.canceled += _ => ButtonRelease();
    }

    void OnEnable()
    {
        controls.Enable();
        GameManager.OnGameStarted += ReadyPlayer;
        GameManager.OnGameOver += Death;
    }

    void OnDisable()
    {
        controls.Disable();
        GameManager.OnGameStarted -= ReadyPlayer;
        GameManager.OnGameOver -= Death;
    }

    void Start()
    {
        if (GameManager.IsFirstGameStart())
        {
            state = PlayerState.Disabled;
        }
        else
        {
            state = PlayerState.AngleSelect;
            angleSelector.StartIndicator();
        }
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
                    // Jump completed
                    OnJumpCompleted?.Invoke();
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
        if (state == PlayerState.AngleSelect)
        {
            NextState();
        }
    }

    public void ButtonRelease()
    {
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
    }

    public PlayerState GetState() { return state; }

    public void Death()
    {
        rb.velocity *= new Vector2(0, rb.velocity.y > 0 ? 0 : 1);
        SetState(PlayerState.Disabled);
    }

    public void ReadyPlayer()
    {
        if (state == PlayerState.Disabled)
        {
            NextState();
        }
    }
}
