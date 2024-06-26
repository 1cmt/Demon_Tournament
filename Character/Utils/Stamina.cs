using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] public int maxStamina = 100;
    [SerializeField] public int curStamina;

    private void Awake()
    {
        maxStamina = 100;
        curStamina = maxStamina;
    }

    public void UseStamina(int amount)
    {
        if (curStamina - amount < 0) return; //이 로직을 카드를 고를 때 적용해야할 것 같다.
        curStamina -= amount;
    }

    public void RecoverStamina(int amount)
    {
        curStamina = Mathf.Min(curStamina + amount, maxStamina);
    }
}
