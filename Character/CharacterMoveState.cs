using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterMoveState : CharacterBaseState
{
    public CharacterMoveState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Move State");
        
        if (stateMachine.character.currentCard is MoveCard moveCard) //혹시 모를 안전 장치
        {
            moveCard.PrintMoveDir();
            StartAnimation(stateMachine.character.AnimationData.moveParameterHash);
            SoundManager.Instance.PlayAudio(Sound.Effect, "Move");
            stateMachine.character.IsMove = true;
            Move(moveCard);
        }
        else
        {
            Debug.Log("Error, This is not Move Card");
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.AnimationData.moveParameterHash);
        stateMachine.character.IsMove = false;
    }

    public override void Update()
    {
        base.Update();
        //TODO : 이동이 모두 끝났는가? 체크해서 IdleState로 돌리기
        //혹시 지나칠수도 있으니 최대한 가까워졌으면 멈추게 합니다.
        if (Vector2.Distance(destination, stateMachine.character.curPosition) < 0.1f)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void Move(MoveCard moveCard)
    {
        //moveCard의 함수에 현재 그리드 좌표를 주어 이동할 그리드 좌표를 얻어옵니다.
        Vector2 newGridPos = moveCard.GetGridPosInfo(stateMachine.character.curGridPos);

        //만일 이동할 그리드 좌표가 (-2, -2) 이라면 이동불가의 의미므로 state를 바꿉니다. 점프스테이트로 하면 좋을거 같은데..
        if (newGridPos != new Vector2(-2, -2))
        {
            stateMachine.character.curGridPos = newGridPos;
            GameManager.Instance.fieldController.ChangeCellColor(newGridPos, Color.green);

            if (stateMachine.character.IsLeft) destination = GameManager.Instance.GetWorldPos(newGridPos, true);
            else destination = GameManager.Instance.GetWorldPos(newGridPos, false);

            dir = (destination - stateMachine.character.curPosition).normalized;
        }
        else
        {
            GameManager.Instance.fieldController.ChangeCellColor(stateMachine.character.curGridPos, Color.green);
            //범위를 벗어낫기에 idle 상태로 돌린다.
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }
}
