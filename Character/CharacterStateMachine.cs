using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : StateMachine
{
    public BaseCharacter character;

    public CharacterIdleState idleState { get; private set; }
    public CharacterMoveState moveState { get; private set; }
    public CharacterAttackState attackState { get; private set; }
    public CharacterDefenseState defenseState { get; private set; }
    public CharacterBuffState buffState { get; private set; }
    public CharacterTakeDamageState takeDamageState { get; private set; }
    public CharacterStateMachine(BaseCharacter character)
    {
        this.character = character;

        idleState = new CharacterIdleState(this);
        moveState = new CharacterMoveState(this);
        attackState = new CharacterAttackState(this);
        defenseState = new CharacterDefenseState(this);
        buffState = new CharacterBuffState(this);
        takeDamageState = new CharacterTakeDamageState(this);
    }
}
