using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDefenseState : CharacterBaseState
{
    public CharacterDefenseState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if(stateMachine.character.currentCard is DefenseCard defenseCard)
        {
            GameManager.Instance.fieldController.ChangeCellColor(stateMachine.character.curGridPos, Color.green);
            stateMachine.character.ReduceDamage = defenseCard.GetReducedDamage();
            stateMachine.character.IsDefenseCard = true;
            StartAnimation(stateMachine.character.AnimationData.defenseParameterHash);
            MakeEffect(defenseCard);
            SoundManager.Instance.PlayAudio(Sound.Effect, "Guard");
        }
        else 
        {
            Debug.Log("Error, This is not Defense Card");
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.AnimationData.defenseParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if(!stateMachine.character.IsDefenseCard)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public void MakeEffect(DefenseCard defenseCard)
    {
        Vector2 worldPos;

        if (stateMachine.character.IsLeft) worldPos = GameManager.Instance.GetWorldPos(stateMachine.character.curGridPos, true, defenseCard.cardData.XOffset);
        else worldPos = GameManager.Instance.GetWorldPos(stateMachine.character.curGridPos, false, defenseCard.cardData.XOffset);

        GameManager.Instance.CreatePrefab(defenseCard.cardData.EffectPrefab, worldPos);
        GameManager.Instance.fieldController.ChangeCellColor(stateMachine.character.curGridPos, Color.green);
    }
}
