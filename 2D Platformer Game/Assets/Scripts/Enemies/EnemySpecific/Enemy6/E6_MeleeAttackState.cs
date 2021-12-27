using UnityEngine;

public class E6_MeleeAttackState : MeleeAttackState
{
    readonly Enemy6 enemy;

    public E6_MeleeAttackState(Enemy6 enemy, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState stateData)
        : base(enemy, stateMachine, animBoolName, attackPosition, stateData)
    {
        this.enemy = enemy;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isAnimationFinished)
            return;

        if (isPlayerMaxAgroRange)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else
        {
            enemy.idleState.SetFlipAfterIdle(false);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void TriggerAttack()
    {
        Collider2D playerObject = Physics2D.OverlapBox(attackPosition.position, attackDetails[meleeAttackType].size, 0f, entity.entityData.player);

        if (playerObject != null)
        {
            attackDetails[meleeAttackType].position = entity.transform.position;
            Player.Instance.Damage(attackDetails[meleeAttackType], enemy, true);
        }
    }

}
