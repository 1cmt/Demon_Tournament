using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CharacterBuffState : CharacterBaseState
{
    public CharacterBuffState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Buff State");

        if (stateMachine.character.currentCard is BuffCard buffCard) //혹시 모를 안전 장치
        {
            Debug.Log("inbuff 현재 스테미나 : " + stateMachine.character.stamina.curStamina);
            stateMachine.character.stamina.RecoverStamina(buffCard.cardData.Stamina);
            Debug.Log("inbuff 회복양 : " + buffCard.cardData.Stamina);
            StartAnimation(stateMachine.character.AnimationData.buffParameterHash);
            MakeEffect(buffCard);
            SoundManager.Instance.PlayAudio(Sound.Effect, "Buff");
        }
        else
        {
            Debug.Log("Error, This is not Buff Card");
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.AnimationData.buffParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (CheckAnimationDone(stateMachine.character.characterData.name + "_Buff"))
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public void MakeEffect(BuffCard buffCard)
    {
        Vector2 worldPos;

        if (stateMachine.character.IsLeft) worldPos = GameManager.Instance.GetWorldPos(stateMachine.character.curGridPos, true, buffCard.cardData.XOffset);
        else worldPos = GameManager.Instance.GetWorldPos(stateMachine.character.curGridPos, false, buffCard.cardData.XOffset);

        GameManager.Instance.CreatePrefab(buffCard.cardData.EffectPrefab, worldPos);
        GameManager.Instance.fieldController.ChangeCellColor(stateMachine.character.curGridPos, Color.blue);
    }
}
