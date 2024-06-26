using System;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private Animator animator;
    private int isBrokenHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnShieldBroken += DestroyShield;
        isBrokenHash = Animator.StringToHash("isBroken");
    }

    private void DestroyShield()
    {
        animator.SetBool(isBrokenHash, true);
        GameManager.Instance.OnShieldBroken -= DestroyShield;
        Destroy(gameObject, 0.5f);
    }
}