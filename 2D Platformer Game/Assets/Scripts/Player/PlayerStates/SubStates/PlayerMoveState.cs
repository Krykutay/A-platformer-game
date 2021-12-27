using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    //int _currentStep;
    //float _lastStepTime = Mathf.NegativeInfinity;
    //float _TimeBetweenSteps = 0.35f;

    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //_currentStep = 0;
        SoundManager.Instance.Play(SoundManager.SoundTags.PlayerRun);
    }

    public override void Exit()
    {
        base.Exit();

        SoundManager.Instance.Stop(SoundManager.SoundTags.PlayerRun);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState)
            return;

        /*
        if (Time.time >= _lastStepTime + _TimeBetweenSteps)
        {
            _lastStepTime = Time.time;
            PlayRunSound();
        } */


        if (xInput == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (yInput == -1)
        {
            stateMachine.ChangeState(player.crouchMoveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.SetVelocityX(playerData.movementVelocity * xInput);
        player.CheckIfShouldFlip(xInput);
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    void PlayRunSound()
    {
        /*
        if (_currentStep == 0)
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerWalk1);
        else if (_currentStep == 1)
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerWalk2);
        else if (_currentStep == 2)
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerWalk3);
        else
        {
            _currentStep = -1;
            SoundManager.Instance.Play(SoundManager.SoundTags.PlayerWalk4);
        }

        _currentStep++;

        */
    }
}
