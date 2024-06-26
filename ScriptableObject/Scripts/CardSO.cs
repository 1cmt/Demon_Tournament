using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Move,
    Attack,
    Defense,
    Buff
}

public enum MoveDirection
{
    None,
    Up,
    Down,
    Right,
    Left
}

public enum AttackShape
{
    None,
    Vertical,
    Horizontal,
    Slash,
    backSlash,
    X, // X 패턴
    Cross, // + 패턴
    TUp, // ㅗ 패턴
    TDown, // ㅜ 패턴
    H, //H 패턴
    LyingH //누워 있는 H패턴
}


[CreateAssetMenu(fileName = "Card", menuName = "New Card")]
public class CardData : ScriptableObject
{
    [field: Header("Info")]
    public CardType cardType;

    [field: SerializeField]
    private int _damage;
    public int Damage
    {
        get { return _damage; }
        private set { _damage = Mathf.Clamp(value, 0, 99); }
    }

    [field: SerializeField]
    private int _stamina;
    public int Stamina
    {
        get { return _stamina; }
        private set { _stamina = Mathf.Clamp(value, 0, 99); }
    }
    [field: SerializeField] public Sprite CardSprite { get; private set; }
    [field: SerializeField] public Sprite CardBackSprite { get; private set; }
    [field: SerializeField] public bool IsUsed { get;  set; }

    [field: Header("Effect")]
    [field: SerializeField] public GameObject EffectPrefab { get; private set; }
    //X는 Y는 파티클서 조정해도 되지만 X의 경우 좌우 케릭터가 다르게 적용되어 불가피한 경우 XOffset을 사용
    [field: SerializeField] public float XOffset { get; private set; } 

    [field: Header("MoveCard")]
    [field: SerializeField] public MoveDirection MoveDir { get; set; }

    [field: Header("AttackCard")]
    [field: SerializeField] public AttackShape AtkShape { get; private set; }
    [field: SerializeField] public float AtkDelay { get; private set; }
    [field: SerializeField] public float TargetDelay { get; private set; }

    [field: Header("AttackSound")]
    [SerializeField] public AttackSound attackSound;

}