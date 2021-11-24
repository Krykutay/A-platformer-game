using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public int facingDirection { get; private set; }
    public int lastDamageDirection { get; private set; }

    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }

    protected bool isStunned;
    protected bool isDead;

    [SerializeField] Transform _wallCheck;
    [SerializeField] Transform _ledgeCheck;
    [SerializeField] Transform _playerCheck;
    [SerializeField] Transform _groundCheck;

    Vector2 _velocityWorkspace;

    float _currentHealth;
    float _currentStunResistance;
    float _lastDamagetime;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        atsm = GetComponent<AnimationToStateMachine>();

        stateMachine = new FiniteStateMachine();
    }

    public virtual void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        _currentHealth = entityData.maxHealth;
        _currentStunResistance = entityData.stunResistance;
        facingDirection = 1;
        isStunned = false;
        isDead = false;
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();

        //anim.SetFloat("yVelocity", rb.velocity.y);

        if (Time.time >= _lastDamagetime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        _velocityWorkspace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = _velocityWorkspace;
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        _velocityWorkspace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = _velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        _currentStunResistance = entityData.stunResistance;
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        _lastDamagetime = Time.time;

        _currentHealth -= attackDetails.damageAmount;
        _currentStunResistance -= attackDetails.stunDamageAmount;

        DamageHop(entityData.damageHopSpeed);

        if (attackDetails.position.x > transform.position.x)
        {
            lastDamageDirection = -1;
        }
        else
        {
            lastDamageDirection = 1;
        }

        if (_currentStunResistance <= 0)
        {
            isStunned = true;
        }

        if (_currentHealth <= 0)
        {
            isDead = true;
        }

        if (!isDead && !isStunned && !CheckPlayerInMaxAgroRange())
        {
            Flip();
        }
    }

    public virtual void DamageHop(float velocity)
    {
        _velocityWorkspace.Set(rb.velocity.x, velocity);
        rb.velocity = _velocityWorkspace;
    }

    public virtual void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, entityData.groundCheckRadius, entityData.ground);
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(_wallCheck.position, transform.right, entityData.wallCheckDistance, entityData.ground);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(_ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.ground);
    }

    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(_playerCheck.position, transform.right, entityData.minAgroDistance, entityData.player);
    }
    
    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(_playerCheck.position, transform.right, entityData.maxAgroDistance, entityData.player);
    }

    public virtual bool CheckPlayerInMeleeRangeAction()
    {
        return Physics2D.Raycast(_playerCheck.position, transform.right, entityData.meleeRangeActionDistance, entityData.player);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(_wallCheck.position, _wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(_ledgeCheck.position, _ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(_playerCheck.position + (Vector3)(Vector2.right * entityData.meleeRangeActionDistance * facingDirection), 0.2f);
        Gizmos.DrawWireSphere(_playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance * facingDirection), 0.2f);
        Gizmos.DrawWireSphere(_playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance * facingDirection), 0.2f);
    }

}
