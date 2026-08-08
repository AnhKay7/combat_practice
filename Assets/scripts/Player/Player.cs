using UnityEngine;

public class Player : MonoBehaviour
{

    #region Movement Config
    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 8f;
    public float MaxMoveSpeed => maxMoveSpeed;

    [SerializeField] private float baseAcceleration = 100f;
    public float BaseAccel => baseAcceleration;

    [SerializeField] private float groundDeceleration = 120f;
    public float GroundDecel => groundDeceleration;

    [SerializeField] private float airDeceleration = 80f;
    public float AirDecel => airDeceleration;

    [SerializeField] private float turnAcceleration = 380f;
    public float TurnAccel => turnAcceleration;
    #endregion

    #region Gravity & Jump Config
    [Header("Gravity & Jump")]
    [SerializeField] private float gravityScale = 5f;
    public float GravityScale => gravityScale;

    [SerializeField] private float fallGravityMultiplier = 1.5f;
    public float FallGravityMultiplier => fallGravityMultiplier;

    [SerializeField] private float jumpHeight = 5f;
    public float JumpHeight => jumpHeight;

    [SerializeField] private float jumpCutMultiplier = 0.5f;
    public float JumpCutMultiplier => jumpCutMultiplier;

    [SerializeField] private float maxFallSpeed = -20f;
    public float MaxFallSpeed => maxFallSpeed;

    [SerializeField] private float coyoteTime = 0.15f;
    public float CoyoteTime => coyoteTime;
    #endregion

    #region Wall Interact Config
    [Header("Wall Interact")]
    [SerializeField] private float wallSlideSpeed = -7f;
    public float WallSlideSpeed => wallSlideSpeed;

    [SerializeField] private float wallCoyoteTime = 0.08f;
    public float WallCoyoteTime => wallCoyoteTime;

    [SerializeField] private float wallJumpForce = 17f;
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
    public PlayerDashController DashController { get; private set; }
    public PlayerCombatController CombatController { get; private set; }
    #endregion

    #region StateMachine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    #endregion

    #region TimeStamp
    public float LastTimeGrounded { get; private set; } = -100f;
    public float LastTimeOnWall { get; private set; } = -100f;
    public void ConsumeGroundedTime() => LastTimeGrounded = -100f;
    public void ConsumeOnWallTime() => LastTimeOnWall = -100f;
    #endregion

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        Movement = GetComponent<KinematicCharacterController>();
        Ground = GetComponent<GroundSensor>();
        Wall = GetComponent<WallSensor>();
        DashController = GetComponent<PlayerDashController>();
        CombatController = GetComponent<PlayerCombatController>();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        FallState = new PlayerFallState(this, StateMachine);
        WallJumpState = new PlayerWallJumpState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, StateMachine);
    }
    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }
    private void Update()
    {
        Input.GetPlayerInput();
        StateMachine.CurrentState.FrameUpdate();
        CombatController.FrameUpdate(Input.IsFacingRight ? 1 : -1);
        HandleActionRequests();
    }
    private void FixedUpdate()
    {
        Ground.CheckGround(Movement.velocityY);
        Wall.CheckWall();

        if (Ground.IsGrounded) LastTimeGrounded = Time.time;
        if (Wall.IsTouchingWall) LastTimeOnWall = Time.time;
        if (Ground.IsGrounded || Wall.IsTouchingWall)
            DashController.ResetAirDashes();

        StateMachine.CurrentState.PhysicUpdate();

        Movement.PhysicsUpdate();
    }
    public void ConsumeAllJumpGraces() //coyote, wall coyote
    {

        ConsumeGroundedTime();
        ConsumeOnWallTime();
        Input.UseJumpInput();
    }
    private void HandleActionRequests()
    {
        if (Input.AttackInput)
        {
            CombatController.TryAttack(StateMachine.CurrentState.CanAttack, Input.IsFacingRight ? 1 : -1);
        }
    }
}
