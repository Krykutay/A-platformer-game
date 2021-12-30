using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    protected D_MeleeAttackState stateData;

    protected AttackDetails[] attackDetails;
    protected float meleeAttackCooldown;

    protected int meleeAttackType = 0;

    float meleeAttackFinishTime = Mathf.NegativeInfinity;
    bool _attackStance;
    int _randInt;

    public MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState stateData)
        : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        entity.CheckIfShouldFlip();

        attackDetails = stateData.attackDetails;
        meleeAttackCooldown = stateData.meleeAttackCooldown;

        _attackStance = false;
        entity.anim.SetBool("meleeAttack", false);
        entity.anim.SetBool("idle", true);
    }

    public override void Exit()
    {
        base.Exit();

        entity.anim.SetBool("meleeAttack", false);
        entity.anim.SetBool("idle", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= meleeAttackFinishTime + meleeAttackCooldown && !_attackStance)
        {
            _attackStance = true;
            entity.anim.SetBool("idle", false);
            entity.anim.SetInteger("meleeAttackType", meleeAttackType);
            entity.anim.SetBool("meleeAttack", true);

            _randInt = Random.Range(0, 3);
            if (_randInt == 0)
                SoundManager.Instance.Play(SoundManager.SoundTags.SkeletonAttack1);
            else if (_randInt == 0)
                SoundManager.Instance.Play(SoundManager.SoundTags.SkeletonAttack2);
            else
                SoundManager.Instance.Play(SoundManager.SoundTags.SkeletonAttack3);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        Collider2D playerObject = Physics2D.OverlapCircle(attackPosition.position, attackDetails[meleeAttackType].attackRadius, entity.entityData.player);

        if (playerObject != null)
        {
            attackDetails[meleeAttackType].position = entity.transform.position;
            Player.Instance.Damage(attackDetails[meleeAttackType], entity, true);
        }
    }

    public override void FinishAttack()
    {
        base.FinishAttack();

        meleeAttackFinishTime = Time.time;

        _attackStance = false;
        entity.anim.SetBool("meleeAttack", false);
        entity.anim.SetBool("idle", true);

        meleeAttackType++;
        if (meleeAttackType >= attackDetails.Length)
            meleeAttackType = 0;
    }
}
