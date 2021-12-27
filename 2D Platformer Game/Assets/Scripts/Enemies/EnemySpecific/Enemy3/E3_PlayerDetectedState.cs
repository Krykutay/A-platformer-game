public class E3_PlayerDetectedState : PlayerDetectedState
{
    readonly Enemy3 enemy;

    public E3_PlayerDetectedState(Enemy3 enemy, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData) 
        : base(enemy, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performMeleeRangeAction)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if (!isDetectingLedge)
        {
            if (!isPlayerInMaxAgroRange)
            {
                enemy.idleState.SetFlipAfterIdle(true);
                stateMachine.ChangeState(enemy.idleState);
            }
            else
            {
                entity.SetVelocityX(0f);
            }
        }
        else if (isPlayerInMinAgroRange && entity.CheckIfPlayerReachableByMeleeAction())
        {
            stateMachine.ChangeState(enemy.chargeState);
        }
        else if (performLongRangeAction && isPlayerInMaxAgroRange && entity.CheckIfPlayerReachableByMeleeAction())
        {
            stateMachine.ChangeState(enemy.chargeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            enemy.idleState.SetFlipAfterIdle(false);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

}
