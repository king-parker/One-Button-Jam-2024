using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    public PlayerControls controls;

    [Header("References")]
    public AngleSelector angleSelector;
    public PowerSelector powerSelector;
    public TimeTracker timeTracker;
    public Rigidbody2D rb;
    public PhysicsMaterial2D defaultMaterial;
    public PhysicsMaterial2D highFrictionMaterial;

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
    private bool isOnPlatform = false;
    private MovingPlatform movingPlatform;
    private float platformSpeedAdjust = 0f;
    private float defaultGravity;

    [Header("SFX Properties")]
    [SerializeField]private float slidingVolumeSpeed = 15f;
    [SerializeField] private float landingVolumeHeight = 15f;
    private AudioSource slidingSFXSource;

    [Header("Other Properties")]
    public float groundCheckDelay = 0.5f;
    public float stopMovingTolerance = 0.01f;

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

        BoxCollider2D bodyCollider = GetComponent<BoxCollider2D>();
        distanceToGround = bodyCollider.bounds.extents.y + bodyCollider.edgeRadius;
        groundCheckLayerMask = LayerMask.GetMask(Layers.WorldName);
        defaultGravity = rb.gravityScale;
    }

    void FixedUpdate()
    {
        // Check if player is on a platform so the platform speed can be considered
        if (isOnPlatform && movingPlatform != null)
        {
            platformSpeedAdjust = movingPlatform.GetVelocity().x;
        }
        else
        {
            platformSpeedAdjust = 0f;
        }

        switch (state)
        {
            case PlayerState.Jumping:
                float checkSpeed = rb.velocity.x > 0 ? rb.velocity.magnitude - platformSpeedAdjust : -rb.velocity.magnitude - platformSpeedAdjust;
                if (checkSpeed > stopMovingTolerance)
                {
                    movedLastUpdate = true;
                }
                else if (checkSpeed <= stopMovingTolerance && !movedLastUpdate && IsGrounded())
                {
                    // Jump completed
                    PlayerData.IncrementPlayerJumps();
                    PlayerData.UpdatePlayerDistance(transform.position.x);
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

        // Adjust sliding SFX volume
        if (slidingSFXSource != null)
        {
            if ((rb.velocity.x - platformSpeedAdjust) > 0)
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
            // Create landing adio
            wasGrounded = true;
            float landingVolume = Mathf.Lerp(0.2f, 1f, peakJumpHeight / landingVolumeHeight);
            SFXManager.instance.PlaySFXClip(landingAudio, this.transform, landingVolume);

            // Create sliding audio
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
                angleSelector.HideIndicator();
                powerSelector.HideIndicator();
                break;
            case PlayerState.AngleSelect:
                // Start timer for decision time stat
                timeTracker.RestartTime();

                angleSelector.StartIndicator();

                // Increase player friction to "stick" player to moving platform
                if (isOnPlatform) { rb.sharedMaterial = highFrictionMaterial; }

                if (slidingSFXSource != null) { Destroy(slidingSFXSource.gameObject); }
                break;
            case PlayerState.PowerSelect:
                angleSelector.StopIndicator();
                powerSelector.StartIndicator();
                break;
            case PlayerState.Jumping:
                // Record decision time to report
                deltaSelectionTime = timeTracker.GetElapsedTime();

                // Make sure the indicators are hidden
                angleSelector.HideIndicator();
                powerSelector.StopIndicator();
                powerSelector.HideIndicator();

                // Communicate with player's rigid body
                rb.AddForce(new Vector2(power * Mathf.Cos(Mathf.Deg2Rad * angle), power * Mathf.Sin(Mathf.Deg2Rad * angle)));
                rb.sharedMaterial = defaultMaterial;
                rb.gravityScale = defaultGravity;

                // Set flags that are checked durring launch state
                movedLastUpdate = true;
                wasGrounded = false;
                allowGroundCheck = false;
                Invoke(nameof(GroundCheckTimerFinished), groundCheckDelay);

                // Manage audio for the launch phase
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
        float playerHalfWidth = distanceToGround;
        Vector3 midPos = transform.position;
        Vector3 leftPos = new (midPos.x - playerHalfWidth, midPos.y);
        Vector3 rightPos = new (midPos.x + playerHalfWidth, midPos.y);

        bool checkLeft = Physics2D.Raycast(leftPos, Vector2.down, distanceToGround + groundedRaycastOffset, groundCheckLayerMask);
        bool checkMid = Physics2D.Raycast(midPos, Vector2.down, distanceToGround + groundedRaycastOffset, groundCheckLayerMask);
        bool checkRight = Physics2D.Raycast(rightPos, Vector2.down, distanceToGround + groundedRaycastOffset, groundCheckLayerMask);

        return (checkLeft || checkMid || checkRight);
    }

    private void GroundCheckTimerFinished()
    {
        allowGroundCheck = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.MovingPlatform))
        {
            // Check if the contact points are for surfaces point upwards
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);

            foreach (var contact in contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.7)
                {
                    isOnPlatform = true;
                    movingPlatform = collision.gameObject.GetComponent<MovingPlatform>();
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.MovingPlatform))
        {
            isOnPlatform = false;
            movingPlatform = null;
        }
    }
}
