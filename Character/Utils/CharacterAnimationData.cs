using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterAnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string moveParmeterName = "Move";
    [SerializeField] private string attackParameterName = "Attack";
    [SerializeField] private string defenseParameterName = "Defense";
    [SerializeField] private string buffParameterName = "Buff";
    [SerializeField] private string takeDamageParameterName = "TakeDamage";



    public int idleParameterHash { get; private set; }
    public int moveParameterHash { get; private set; }
    public int attackParameterHash { get; private set; }
    public int defenseParameterHash { get; private set; }
    public int buffParameterHash { get; private set; }
    public int takeDamageParameterHash { get; private set; }




    public void Initialize()
    {
        idleParameterHash = Animator.StringToHash(idleParameterName);
        moveParameterHash = Animator.StringToHash(moveParmeterName);
        attackParameterHash = Animator.StringToHash(attackParameterName);
        defenseParameterHash = Animator.StringToHash(defenseParameterName);
        buffParameterHash = Animator.StringToHash(buffParameterName);
        takeDamageParameterHash = Animator.StringToHash(takeDamageParameterName);
    }
}
