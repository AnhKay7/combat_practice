using UnityEngine;

public class Player : MonoBehaviour
{

    #region Movement Config
    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 8f;
    public float MaxMoveSpeed => maxMoveSpeed;

    [SerializeField] private float groundAcceleration = 100f;
    public float GroundAccel => groundAcceleration;

    [SerializeField] private float groundDeceleration = 120f;
    public float GroundDecel => groundDeceleration;

    [SerializeField] private float airDeceleration = 50f;
    public float AirDecel => airDeceleration;

    [SerializeField] private float turnAcceleration = 380f;
    public float TurnAccel => turnAcceleration;
    #endregion

    #region Gravity & Jump Config
    [Header("Gravity & Jump")]
    [SerializeField] private float gravityScale = 5f;
    public float GravityScale => gravityScale;

    [SerializeField] private float jumpHeight = 5f;
    public float JumpHeight => jumpHeight;

    [SerializeField] private float jumpCutMultiplier = 0.5f;
    public float JumpCutMultiplier => jumpCutMultiplier;

    [SerializeField] private float maxFallSpeed = -20f;
    public float MaxFallSpeed => maxFallSpeed;

    [SerializeField] private float coyoteTime = 0.15f;
    public float CoyoteTime => coyoteTime;
    #endregion

    #region Dash Config
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 25f;
    public float DashSpeed => dashSpeed;

    [SerializeField] private float dashDuration = 0.2f;
    public float DashDuration => dashDuration;

    [SerializeField] private float dashCooldown = 0.4f;
    public float DashCooldown => dashCooldown;
    #endregion

    #region Wall Interact Config
    [Header("Wall Interact")]
    [SerializeField] private float wallSlideSpeed = -5f;
    public float WallSlideSpeed => wallSlideSpeed;

    [SerializeField] private float wallCoyoteTime = 0.08f;
    public float WallCoyoteTime => wallCoyoteTime;

    [SerializeField] private float wallJumpForce = 12f;
    public float WallJumpForce => wallJumpForce;

    [SerializeField] private float wallJumpDuration = 0.15f;
    public float WallJumpDuration => wallJumpDuration;

    [SerializeField] private float wallJumpAirControl = 0.05f;
    public float WallJumpAirControl => wallJumpAirControl;
    #endregion

    #region Component
    public PlayerInput Input { get; private set; }
    public KinematicCharacterController Movement { get; private set; }

    public GroundSensor Ground { get; private set; }
    public WallSensor Wall { get; private set; }
    #endregion

    #region StateMachine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    #endregion

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        Movement = GetComponent<KinematicCharacterController>();
        Ground = GetComponent<GroundSensor>();
        Wall = GetComponent<WallSensor>();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
    }
    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }
    private void Update()
    {
        Input.GetPlayerInput();
        StateMachine.CurrentState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        Ground.CheckGround(Movement.velocity_y);
        Wall.CheckWall();

        StateMachine.CurrentState.PhysicUpdate();

        Movement.PhysicsUpdate();
    }
}
