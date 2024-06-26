using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int curHealth;
    public event Action OnDeath;

    private void Awake()
    {
        maxHealth = 100;
        curHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        curHealth -= amount;
        if (curHealth < 0)
        {
            curHealth = 0;
            OnDeath?.Invoke();
        }
    }

}
