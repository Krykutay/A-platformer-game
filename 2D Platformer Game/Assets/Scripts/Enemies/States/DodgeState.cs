using UnityEngine;

public class DodgeState : State
{
    protected D_DodgeState stateData;

    protected bool performedCloseRangeAction;
    protected bool isPlayerInMaxAgroRange;
    protected bool isGrounded;
    protected bool isDodgeOver;

    bool _isLedgeBehind;
    bool _isLedgeDetectionActionTaken;

    public DodgeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DodgeState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        isDodgeOver = false;
        _isLedgeDetectionActionTaken = false;
        entity.SetVelocity(stateData.dodgeSpeed, stateData.dodgeAngle, -entity.facingDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!_isLedgeBehind && !_isLedgeDetectionActionTaken)
        {
            entity.SetVelocityX(0f);
            _isLedgeDetectionActionTaken = true;
        }

        if (Time.time >= startTime + stateData.dodgeTime && isGrounded)
        {
            isDodgeOver = true;
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();

        performedCloseRangeAction = entity.CheckPlayerInMeleeRangeAction();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();
        isGrounded = entity.CheckGround();

        if (!_isLedgeDetectionActionTaken)
            _isLedgeBehind = entity.CheckLedgeBehind();
    }

    public bool CheckCanDodge(Vector3 position)
    {
        bool isLedge = !Physics2D.Raycast(position, Vector2.down, entity.entityData.ledgeCheckDistance, entity.entityData.ground);
        bool isWall = Physics2D.Raycast(position, -Vector2.right, entity.entityData.wallCheckDistance, entity.entityData.ground);

        return (!isLedge && !isWall);
    }

}
