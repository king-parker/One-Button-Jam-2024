using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour, IPlayer
{
    public PlayerControls controls;

    [Header("References")]
    public AngleSelector angleSelector;
    public PowerSelector powerSelector;
    public TimeTracker timeTracker;
    public Rigidbody2D rb;
    public Collider2D bodyCollider;

    [Header("SFX Audio")]
    public AudioClip jumpAudio;
    public AudioClip landingAudio;
    public AudioClip slidingAudio;

    public static event Action OnJumpCompleted;

    private float angle = 0;
    private float power = 0;
    private PlayerState state;
    private float distanceToGround = 0f;
    private float groundedRaycastOffset = 0.3f;
    private bool movedLastUpdate = false;
    private bool wasGrounded = true;
    private bool allowGroundCheck = false;
    private int groundCheckLayerMask = 0;
    private float peakJumpHeight = 0f;
    private float deltaSelectionTime = 0f;

    [Header("SFX Properties")]
    [SerializeField]private float slidingVolumeSpeed = 15f;
    [SerializeField] private float landingVolumeHeight = 15f;
    private AudioSource slidingSFXSource;

    [Header("Other Properties")]
    public float groundCheckDelay = 0.5f;

    public enum PlayerState
    {
        Disabled,
        AngleSelect,
        PowerSelect,
        Jumping
    }

    void Awake()
    {
        controls ??= new PlayerControls();
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

        distanceToGround = bodyCollider.bounds.extents.y;
        groundCheckLayerMask = LayerMask.GetMask(Layers.WorldName);
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Jumping:
                if (rb.velocity.magnitude > 0)
                {
                    movedLastUpdate = true;
                }
                else if (rb.velocity.magnitude == 0 && !movedLastUpdate)
                {
                    // Jump completed
                    PlayerData.IncrementPlayerJumps();
                    PlayerData.SetPlayerDistance(transform.position.x);
                    PlayerData.AddSelectionTime(deltaSelectionTime);
                    OnJumpCompleted?.Invoke();
                    NextState();
                }
                else
                {
                    movedLastUpdate = false;
                }

                if (transform.position.y > peakJumpHeight) { peakJumpHeight = transform.position.y; }
                break;
        }

        if (slidingSFXSource != null)
        {
            if (rb.velocity.magnitude > 0)
            {
                if (IsGrounded())
                {
                    slidingSFXSource.volume = rb.velocity.magnitude / slidingVolumeSpeed;
                }
                else
                {
                    slidingSFXSource.volume = 0;
                }
            }
            else
            {
                Destroy(slidingSFXSource.gameObject);
            }
        }

        // Outside of jumping case so sound is played when in disabled state
        if (allowGroundCheck && !wasGrounded && IsGrounded())
        {
            wasGrounded = true;
            float landingVolume = Mathf.Lerp(0.2f, 1f, peakJumpHeight / landingVolumeHeight);
            SFXManager.instance.PlaySFXClip(landingAudio, this.transform, landingVolume);

            if (rb.velocity.magnitude > 0 && slidingSFXSource == null)
            {
                slidingSFXSource = SFXManager.instance.CreateContinuousSFXClip(slidingAudio, this.gameObject, 1f);
                slidingSFXSource.Play();
            }
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
                timeTracker.RestartTime();
                angleSelector.StartIndicator();
                break;
            case PlayerState.PowerSelect:
                angleSelector.StopIndicator();
                powerSelector.StartIndicator();
                break;
            case PlayerState.Jumping:
                deltaSelectionTime = timeTracker.GetElapsedTime();
                angleSelector.HideIndicator();
                powerSelector.StopIndicator();
                powerSelector.HideIndicator();
                rb.AddForce(new Vector2(power * Mathf.Cos(Mathf.Deg2Rad * angle), power * Mathf.Sin(Mathf.Deg2Rad * angle)));
                movedLastUpdate = true;
                wasGrounded = false;
                allowGroundCheck = false;
                Invoke(nameof(GroundCheckTimerFinished), groundCheckDelay);
                peakJumpHeight = 0;
                float launchVolume = Mathf.Lerp(0f, .8f, power / powerSelector.maxPower);
                SFXManager.instance.PlaySFXClip(jumpAudio, this.transform, launchVolume);
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

    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, distanceToGround + groundedRaycastOffset, groundCheckLayerMask);
    }

    private void GroundCheckTimerFinished()
    {
        allowGroundCheck = true;
    }
}
