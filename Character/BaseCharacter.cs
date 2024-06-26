using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [field: SerializeField] public CharacterSO characterData { get; set; }
    [field: SerializeField] public CharacterAnimationData AnimationData { get; private set; }

    public Health health { get; set; }
    public Stamina stamina { get; set; }
    public int ReduceDamage { get; set; }
    //private int stageNum;    
    private int _damage;
    public int Damage { get { return _damage; } set { if (value > 0) Damage = value; } }
    public bool IsDefenseCard { get; set; }
    public bool IsMove { get; set; } = false;
     public bool IsLeft { get; set; }

    //현재 캐릭터의 위치를 나타냄
    [SerializeField] public Vector2 curPosition { get; set; }
    public Vector2 curGridPos;
    public Vector2 nextCoord;

    //전투시 3개의 카드를 담을 큐
    [SerializeField] public Queue<Card> cardQueue = new Queue<Card>();
    //캐릭터가 가지는 고유의 카드리스트
    [SerializeField] public List<GameObject> cardList;
    public Card currentCard;

    public CharacterStateMachine characterStateMachine;
    [field: SerializeField] public Animator animator { get; set; }
    public Rigidbody2D rb;

    private void Awake()
    {
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        AnimationData.Initialize();
        characterStateMachine = new CharacterStateMachine(this);
        
    }
    private void Start()
    {
        //InitCharacterPosition(); << 게임매니저에서 잡아줍니다.
        characterStateMachine.ChangeState(characterStateMachine.idleState);
        health.OnDeath += DeathAnimation;
    }
    private void Update()
    {
        characterStateMachine.Update();
    }
    private void FixedUpdate()
    {
        characterStateMachine.PhysicsUpdate();
        curPosition = transform.position;
    }

    private void DeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}
