using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public static Action PlayerDied;
    public static Player Instance { get; private set; }

    [SerializeField] PlayerData _playerData;

    [SerializeField] Transform _groundCheck;
    [SerializeField] Transform _wallCheck;
    [SerializeField] Transform _verticalLedgeCheck;
    [SerializeField] Transform _ceilingCheck;
    [SerializeField] Transform _playerHitPosition;

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerLandState landState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallGrabState wallGrabState { get; private set; }
    public PlayerWallClimbState wallClimbState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerLedgeClimbState ledgeClimbState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerCrouchIdleState crouchIdleState { get; private set; }
    public PlayerCrouchMoveState crouchMoveState { get; private set; }
    public PlayerAttackState primaryAttackState { get; private set; }
    public PlayerAttackState secondaryAttackState { get; private set; }

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public BoxCollider2D movementCollider { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public Transform dashDirectionIndicator { get; private set; }
    public PlayerInventory inventory { get; private set; }

    public Vector2 currentVelocity { get; private set; }
    public int facingDirection { get; private set; }
    public float initialGravity { get; private set; }

    Vector2 _workSpace;
    float _currentHealth;
    float _initialHitPositionY;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("There can not be two player instances at once");

        anim = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();
        movementCollider = GetComponent<BoxCollider2D>();
        inventory = GetComponent<PlayerInventory>();
        dashDirectionIndicator = transform.Find("DashDirectionIndicator");

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, _playerData, "idle");
        moveState = new PlayerMoveState(this, stateMachine, _playerData, "move");
        jumpState = new PlayerJumpState(this, stateMachine, _playerData, "inAir");
        inAirState = new PlayerInAirState(this, stateMachine, _playerData, "inAir");
        landState = new PlayerLandState(this, stateMachine, _playerData, "land");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, _playerData, "wallSlide");
        wallGrabState = new PlayerWallGrabState(this, stateMachine, _playerData, "wallGrab");
        wallClimbState = new PlayerWallClimbState(this, stateMachine, _playerData, "wallClimb");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, _playerData, "inAir");
        ledgeClimbState = new PlayerLedgeClimbState(this, stateMachine, _playerData, "ledgeClimbState");
        dashState = new PlayerDashState(this, stateMachine, _playerData, "inAir");
        crouchIdleState = new PlayerCrouchIdleState(this, stateMachine, _playerData, "crouchIdle");
        crouchMoveState = new PlayerCrouchMoveState(this, stateMachine, _playerData, "crouchMove");
        primaryAttackState = new PlayerAttackState(this, stateMachine, _playerData, "attack");
        secondaryAttackState = new PlayerAttackState(this, stateMachine, _playerData, "attack");
    }

    void OnEnable()
    {
        primaryAttackState.SetWeapon(inventory.weapons[0]);
        //secondaryAttackState.SetWeapon(inventory.weapons[(int)CombatInputs.primary]);

        stateMachine.Initialize(idleState);
        facingDirection = 1;
        _currentHealth = _playerData.maxHealth;
        initialGravity = rb.gravityScale;
    }

    void Start()
    {
        _initialHitPositionY = _playerHitPosition.localPosition.y;
    }

    void Update()
    {
        currentVelocity = rb.velocity;
        stateMachine.currentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public void SetVelocityZero()
    {
        rb.velocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }

    public void SetVelocityX(float velocity)
    {
        _workSpace.Set(velocity, currentVelocity.y);
        rb.velocity = _workSpace;
        currentVelocity = _workSpace;
    }

    public void SetVelocityY(float velocity)
    {
        _workSpace.Set(currentVelocity.x, velocity);
        rb.velocity = _workSpace;
        currentVelocity = _workSpace;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        _workSpace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = _workSpace;
        currentVelocity = _workSpace;
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        _workSpace = direction * velocity;
        rb.velocity = _workSpace;
        currentVelocity = _workSpace;
    }

    public void SetPlayerHitPositionInCrouch()
    {
        _playerHitPosition.localPosition = new Vector3(0f, _playerHitPosition.localPosition.y * 2, 0f);
    }

    public void ResetPlayerHitPosition()
    {
        _playerHitPosition.localPosition = new Vector3(0f, _initialHitPositionY, 0f);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
        {
            Flip();
        }
    }

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, _playerData.groundCheckRadius, _playerData.ground);
    }

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(_wallCheck.position, Vector2.right * facingDirection, _playerData.wallCheckDistance, _playerData.ground);
    }

    public bool CheckIfTouchingWallBack()
    {
        return Physics2D.Raycast(_wallCheck.position, Vector2.right * -facingDirection, _playerData.wallCheckDistance, _playerData.ground);
    }

    public bool CheckIfTouchingVerticalLedge()
    {
        return Physics2D.Raycast(_verticalLedgeCheck.position, Vector2.right * facingDirection, _playerData.wallCheckDistance, _playerData.ground);
    }

    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(_ceilingCheck.position, _playerData.groundCheckRadius, _playerData.ground);
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(
            _wallCheck.position,
            Vector2.right * facingDirection, _playerData.wallCheckDistance,
            _playerData.ground);
        float xDist = xHit.distance;
        _workSpace.Set((xDist + 0.015f) * facingDirection, 0f);

        RaycastHit2D yHit = Physics2D.Raycast(
            _verticalLedgeCheck.position + (Vector3)_workSpace,
            Vector2.down, _verticalLedgeCheck.position.y - _wallCheck.position.y + 0.015f,
            _playerData.ground);
        float yDist = yHit.distance;
        _workSpace.Set(_wallCheck.position.x + xDist * facingDirection, _verticalLedgeCheck.position.y - yDist);

        return _workSpace;
    }

    public void SetColliderHeight(float height)
    {
        Vector2 center = movementCollider.offset;
        _workSpace.Set(movementCollider.size.x, height);

        center.y += (height - movementCollider.size.y) / 2; 

        movementCollider.size = _workSpace;
        movementCollider.offset = center;
    }

    public bool Damage(AttackDetails attackDetails)
    {
        if (stateMachine.currentState == dashState)
            return false;

        DecreaseHealth(attackDetails.damageAmount);

        /*
        int enemyDirection;

        if (attackDetails.position.x < transform.position.x)
        {
            enemyDirection = 1;
        }
        else
        {
            enemyDirection = -1;
        }

        // maybe knockback?
        */

        return true;
    }

    public void DecreaseHealth(float amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        DeathChunkParticlePool.Instance.Get(transform.position, Quaternion.Euler(0f, 0f, 0f));
        DeathBloodParticlePool.Instance.Get(transform.position, Quaternion.Euler(0f, 0f, 0f));
        PlayerDied?.Invoke();
        Destroy(gameObject);
    }

    void AnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    void AnimationFinishTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(_wallCheck.position, new Vector3(_wallCheck.position.x + _playerData.wallCheckDistance, _wallCheck.position.y, _wallCheck.position.z));
        Gizmos.DrawLine(_verticalLedgeCheck.position, new Vector3(_verticalLedgeCheck.position.x + _playerData.wallCheckDistance, _verticalLedgeCheck.position.y, _verticalLedgeCheck.position.z));

        Gizmos.DrawWireSphere(_ceilingCheck.position, _playerData.groundCheckRadius);
    }

}
