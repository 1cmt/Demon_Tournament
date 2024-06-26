using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBaseState : IState
{
    protected CharacterStateMachine stateMachine;
    protected CharacterSO characterData;
    protected Vector2 destination; //캐릭터가 도착해야하는 방향
    protected Vector2 dir;

    public CharacterBaseState(CharacterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        characterData = stateMachine.character.characterData;
    }
    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
        //Move
        if (stateMachine.character.IsMove)
        {
            stateMachine.character.rb.velocity = dir * 4f;
        }
        //Not Move
        else if (!stateMachine.character.IsMove)
        {
            stateMachine.character.rb.velocity = Vector2.zero;
        }
    }

    protected void StartAnimation(int animatorHash)
    {
        stateMachine.character.animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.character.animator.SetBool(animatorHash, false);
    }

    protected bool CheckAnimationDone(string animationName) //애니메이션이아니라 파티클을 끝내야할수도? 아니면 애니메이션과 파티클의 실행시간을 동일하게 만들거나
    {
        if (stateMachine.character.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            if (stateMachine.character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                return true;
            }
        }
        return false;
    }

    
}
