using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    WeaponData _weaponData;
    Transform _attackPosition;

    int _attackCounter;
    float _comboStartTime = Mathf.NegativeInfinity;

    int _xInput;
    bool _jumpInput;
    bool _defenseInput;

    bool _isTouchingCeiling;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName, WeaponData weaponData, Transform attackPosition) :
        base(player, stateMachine, playerData, animBoolName)
    {
        _weaponData = weaponData;
        _attackPosition = attackPosition;
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;

        if (_attackCounter >= _weaponData.amountOfAttacks)
        {
            _attackCounter = 0;
        }
        else if (Time.time >= _comboStartTime + _weaponData.comboDuration)
        {
            int randomAttackCounter = Random.Range(0, 2);
            if (randomAttackCounter == 0)
                _attackCounter = 0;
            else
                _attackCounter = 3;
        }

        _comboStartTime = Time.time;
        player.anim.SetInteger("attackCounter", _attackCounter);

        if (_attackCounter == 0 || _attackCounter == 3)
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerMelee1);
        else if (_attackCounter == 1 || _attackCounter == 4)
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerMelee2);
        else
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerMelee3);
    }

    public override void Exit()
    {
        base.Exit();

        _attackCounter++;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        _xInput = player.inputHandler.xInput;
        _jumpInput = player.inputHandler.jumpInput;
        _defenseInput = player.inputHandler.defenseInput;

        if (_jumpInput && player.jumpState.CanJump() && !_isTouchingCeiling)
        {
            AnimationFinishTrigger();
            stateMachine.ChangeState(player.jumpState);
        }
        else if (_defenseInput)
        {
            AnimationFinishTrigger();
            stateMachine.ChangeState(player.defenseState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.CheckIfShouldFlip(_xInput);
        player.SetVelocityX(_weaponData.movementSpeed * _xInput);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        _isTouchingCeiling = player.CheckForCeiling();
    }

    void CheckMeleeAttack()
    {
        AttackDetails attackDetails = _weaponData.attackDetails[_attackCounter];
        attackDetails.position = player.transform.position;

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(_attackPosition.position, attackDetails.attackRadius, _weaponData.damageable);
        bool isScreenShaked = false;

        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                bool isHit = damageable.Damage(attackDetails);

                if (!isScreenShaked && isHit)
                {
                    SoundManager.Instance.Play(SoundManager.SoundTags.SkeletonHurt);
                    CinemachineShake.Instance.ShakeCamera(1.5f, 0.1f);
                    isScreenShaked = true;
                }
            }
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        CheckMeleeAttack();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }
}
