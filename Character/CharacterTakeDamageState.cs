using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTakeDamageState : CharacterBaseState
{
    public CharacterTakeDamageState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.character.AnimationData.takeDamageParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.AnimationData.takeDamageParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (CheckAnimationDone(stateMachine.character.characterData.name + "_TakeDamage"))
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }
}
