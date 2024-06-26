using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CharacterAttackState : CharacterBaseState
{
    public CharacterAttackState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (stateMachine.character.currentCard is AttackCard attackCard)
        {
            StartAnimation(stateMachine.character.AnimationData.attackParameterHash);
            //카드SO에 음악을 넣으면 무기별 다른 사운드가 가능할거같네요.
            List<Vector2> gridPosList = attackCard.GetGridPosList(stateMachine.character.curGridPos); //이팩트가 생성될 좌표를 가져왔습니다.
            MakeEffect(attackCard, gridPosList);
            SoundManager.Instance.PlayAudio(Sound.Effect, attackCard.cardData.attackSound.ToString());
        }
        else
        {
            Debug.Log("Error, This is not Attack Card");
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.AnimationData.attackParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (CheckAnimationDone(stateMachine.character.characterData.name + "_Attack"))
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public void MakeEffect(AttackCard attackCard, List<Vector2> gridPosList)
    {
        Debug.Log("이팩트 생성");

        foreach (Vector2 gridPos in gridPosList)
        {
            GameManager.Instance.fieldController.ChangeCellColor(gridPos, Color.red);
            Vector2 worldPos;

            //적의 X 위치에 따라 이팩트 생성 위치를 다르게하여 케릭터 바로 위에 생성될 수 있도록 한다. (왼쪽 캐는 왼쪽 끝에 오른쪽 캐는 오른쪽 끝에 위치하기에)
            if (GameManager.Instance.currentEnemy.IsLeft) worldPos = GameManager.Instance.GetWorldPos(gridPos, true, attackCard.cardData.XOffset);
            else worldPos = GameManager.Instance.GetWorldPos(gridPos, false, attackCard.cardData.XOffset);

            GameManager.Instance.CreatePrefab(attackCard.cardData.EffectPrefab, worldPos, attackCard.cardData.AtkDelay);

            //만일 적의 그리드 좌표와 이팩트의 그리드 좌표가 일치하면 공격을 받도록
            if (GameManager.Instance.currentEnemy.curGridPos == gridPos)
            {
                GameManager.Instance.ApplyDamage(attackCard.cardData.Damage, attackCard.cardData.TargetDelay);
            }
        }
    }
}
