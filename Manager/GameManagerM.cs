using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameManagerM : MonoBehaviourPunCallbacks
{
    private static GameManagerM _instance;
    public static GameManagerM Instance { get { return _instance; } }

    public FieldController fieldController;
    public float Xoffset = -1.0f;
    public float _Xoffset = 1.0f;
    public float Yoffset = 0.3f;

    public BaseCharacter currentCharacter;
    public BaseCharacter currentEnemy;

    public enum GameState
    {
        CardSelect, //카드 선택
        Player1Turn,
        Player2Turn,
        Player1Win,
        Player2Win
    }

    public GameState state;
    public GameState firstTurn; //처음 시작할 턴

    public BaseCharacter player1Character;
    public BaseCharacter player2Character;
    public List<BaseCharacter> _characters;

    public int stageNum;

    public int player1Damage;
    public int player2Damage;
    public int player1Stamina;
    public int player2Stamina;
    public int cardsPlayedByPlayer1;
    public int cardsPlayedByPlayer2;

    public GameObject cardSelectUI;
    public GameObject matchView;
    public GameObject conditionUI;
    public GameObject popupWin;
    public GameObject popupLose;

    public bool hasFighted = false;
    public bool hasMovedScene = false;
    public bool isPaused = false;

    [SerializeField] private float moveWait = 3f;
    [SerializeField] private float defenseWait = 2.5f;
    [SerializeField] private float buffWait = 2.5f;
    [SerializeField] private float attackWait = 3f;
    [SerializeField] private float updateHpBarWait = 2.5f;

    public event Action OnChangeHP;
    public event Action OnChangeSP;
    public event Action OnShieldBroken;

    public MainSceneUtils utils;

    PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        firstTurn = GameState.Player1Turn; // 처음엔 Player 1 부터
        hasFighted = false;

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("포톤: 어림없지");
        }
    }

    private IEnumerator ChangeGameState(GameState state)
    {
        yield return null; //호출한 함수가 종료될 수 있게
        Debug.Log(state.ToString() + " 시작");

        this.state = state;
        switch (state)
        {
            case GameState.CardSelect:
                //카드선택 상태에선 카드선택 UI가 보이도록 합니다.
                ShowCardSelectUI();
                break;
            case GameState.Player1Turn:
                currentCharacter = player1Character;
                currentEnemy = player2Character;
                cardsPlayedByPlayer1++;
                StartCoroutine(PlayCard());
                break;
            case GameState.Player2Turn:
                currentCharacter = player2Character;
                currentEnemy = player1Character;
                cardsPlayedByPlayer2++;
                StartCoroutine(PlayCard());
                break;
            case GameState.Player1Win:
                SetPlayer1Win(player1Character);
                break;
            case GameState.Player2Win:
                SetPlayer2Win(player2Character);
                break;
            default:
                break;
        }
    }

    void SetInitialPositions()
    {
        // 초기 위치 설정
        Debug.Log("초기 위치 설정");
        fieldController.MovePlayerToCell(player1Character.transform, fieldController.playerGridPosition, Xoffset, Yoffset);
        fieldController.MovePlayerToCell(player2Character.transform, fieldController.aiGridPosition, _Xoffset, Yoffset);
        player2Character.transform.rotation = Quaternion.Euler(0, 180f, 0);

        player1Character.curGridPos = fieldController.playerGridPosition;
        player2Character.curGridPos = fieldController.aiGridPosition;
    }

    void InitData(bool isRoundEnd)
    {
        player1Damage = 0;
        player2Damage = 0;
        player1Stamina = 0;
        player2Stamina = 0;
        if (isRoundEnd)
        {
            cardsPlayedByPlayer1 = 0;
            cardsPlayedByPlayer2 = 0;
        }
    }

    public Vector3 GetWorldPos(Vector2 gridPos, bool isLeft)
    {
        if (isLeft) return fieldController.GetWorldPos(gridPos, Xoffset, Yoffset);
        else return fieldController.GetWorldPos(gridPos, _Xoffset, Yoffset);
    }

    public Vector3 GetWorldPos(Vector2 gridPos, bool isLeft, float xOffset)
    {
        if (isLeft) return fieldController.GetWorldPos(gridPos, -xOffset, Yoffset);
        else return fieldController.GetWorldPos(gridPos, xOffset, Yoffset);
    }

    public void CreatePrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        photonView.RPC("RPC_CreatePrefab", RpcTarget.All, prefab.name, position, rotation);
    }

    [PunRPC]
    public void RPC_CreatePrefab(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            Instantiate(prefab, position, rotation);
        }
        else
        {
            Debug.LogError("Prefab not found: " + prefabName);
        }
    }

    public void StartStage()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
        }

        utils = GameObject.FindGameObjectWithTag("Utils").GetComponent<MainSceneUtils>();
        fieldController = utils.GetComponent<FieldController>();
        cardSelectUI = utils.cardSelectUI;
        matchView = utils.matchView;
        conditionUI = utils.conditionUI;
        popupWin = utils.popupWin;
        popupLose = utils.popupLose;

        //Instantiate(player1Character.gameObject);
        //Instantiate(player2Character.gameObject);
        
        // 플레이어 1과 플레이어 2의 프리팹을 Photon PUN 2로 인스턴스화
        GameObject player1Obj = PhotonNetwork.Instantiate("Player1PrefabName", Vector3.zero, Quaternion.identity);
        GameObject player2Obj = PhotonNetwork.Instantiate("Player2PrefabName", Vector3.zero, Quaternion.identity);

        player1Character = player1Obj.GetComponent<BaseCharacter>();
        player2Character = player2Obj.GetComponent<BaseCharacter>();

        // 마스터 클라이언트 여부에 따라 초기 위치와 방향 설정
        if (PhotonNetwork.IsMasterClient)
        {
            player1Character.IsLeft = true;
            player2Character.IsLeft = false;
        }
        else
        {
            player1Character.IsLeft = false;
            player2Character.IsLeft = true;
        }

        InitData(true); // 데이터 초기화
        SetInitialPositions(); // 플레이어 위치 초기화

        // 설정 초기화 후 카드 선택 상태를 수행합니다.
        StartCoroutine(ChangeGameState(GameState.CardSelect));
    }

    private void ShowCardSelectUI()
    {
        cardSelectUI.SetActive(true);

        // cardSelectUI 초기화
        cardSelectUI.GetComponent<UICardView>().OnClickClearButton();
    }

    private void SetPlayer1Win(BaseCharacter winningCharacter)
    {
        Time.timeScale = 0;
        isPaused = true;
        stageNum++;
        PhotonNetwork.Destroy(player1Character.gameObject);
        PhotonNetwork.Destroy(player2Character.gameObject);
        if (winningCharacter == player1Character)
        {
            popupWin.SetActive(true);
        }
        else
        {
            popupLose.SetActive(true);
        }
    }

    private void SetPlayer2Win(BaseCharacter winningCharacter)
    {
        Time.timeScale = 0;
        isPaused = true;
        stageNum++;
        PhotonNetwork.Destroy(player1Character.gameObject);
        PhotonNetwork.Destroy(player2Character.gameObject);
        if (winningCharacter == player2Character)
        {
            popupWin.SetActive(true);
        }
        else
        {
            popupLose.SetActive(true);
        }
    }

    //카드 선택 UI에서 Continue를 클릭시 해당함수를 수행하도록 하면 됩니다.

    public void Continue()
    {
        //firstTurn엔 먼저 시작할 캐릭터의 턴이 담겨있습니다.
        StartCoroutine(ChangeGameState(firstTurn));

        photonView.RPC("RPC_Continue", RpcTarget.Others, (int)firstTurn);
    }

    [PunRPC]
    void RPC_Continue(int firstTurnValue)
    {
        firstTurn = (GameState)firstTurnValue;
    }

    private IEnumerator PlayCard()
    {
        yield return null; //호출한 코루틴 함수는 종료될 수 있게

        Debug.Log("카드 뽑기");
        Card card = currentCharacter.cardQueue.Dequeue().GetComponent<Card>();
        currentCharacter.currentCard = card;
        float time = 0;

        // 카드 애니메이션 실행
        if (currentCharacter == player1Character)
        {
            utils.playerCards[cardsPlayedByPlayer1 - 1].UseCard();
        }
        else
        {
            utils.enemyCards[cardsPlayedByPlayer2 - 1].UseCard();
        }

        yield return new WaitForSeconds(1.5f);

        switch (card.GetCardType())
        {
            case CardType.Move:
                MoveCard moveCard = card as MoveCard;
                if (moveCard != null)
                {
                    ChangeCharacterStateToMoveState();
                    time = moveWait;
                }
                break;
            case CardType.Defense:
                DefenseCard defenseCard = card as DefenseCard;
                if (defenseCard != null)
                {
                    ChangeCharacterStateToDefenseState();
                    time = defenseWait;
                }
                break;
            case CardType.Buff:
                BuffCard buffCard = card as BuffCard;
                if (buffCard != null)
                {
                    ChangeCharacterStateToBuffState();
                    time = buffWait;
                }
                break;
            case CardType.Attack:
                AttackCard attackCard = card as AttackCard;
                if (attackCard != null)
                {
                    ChangeCharacterStateToAttackState(attackCard);
                    time = attackWait;
                }
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(time);
        Debug.Log("Player 1 턴 수 : " + cardsPlayedByPlayer1 + " Player 2 턴 수 : " + cardsPlayedByPlayer2);
        if (cardsPlayedByPlayer1 == cardsPlayedByPlayer2) StartCoroutine(EndBothTurn()); // 둘이 카드 꺼낸 횟수가 같으면 서로의 턴이 종료된 것입니다.
        else BeginNextTurn(); // 횟수가 다르면 상대 턴이 남아 있는 것임
    }

    private IEnumerator EndBothTurn()
    {
        Debug.Log("둘다 턴 끝!");
        player2Character.health.TakeDamage(player1Damage);
        player1Character.health.TakeDamage(player2Damage);

        player1Character.stamina.UseStamina(player1Stamina);
        player2Character.stamina.UseStamina(player2Stamina);

        Debug.Log("Player 1 SM: " + player1Character.stamina.curStamina + " Player 2 SM: " + player2Character.stamina.curStamina);

        player1Character.IsDefenseCard = false;
        player2Character.IsDefenseCard = false;

        OnChangeHP?.Invoke();
        OnChangeSP?.Invoke();

        if (player1Damage != 0 || player2Damage != 0) yield return new WaitForSeconds(updateHpBarWait);

        Debug.Log("Player 1 HP:" + player1Character.health.curHealth + ", Player 2 HP: " + player2Character.health.curHealth);

        if (player2Character.health.curHealth <= 0) StartCoroutine(ChangeGameState(GameState.Player1Win));
        else if (player1Character.health.curHealth <= 0) StartCoroutine(ChangeGameState(GameState.Player2Win));

        if (cardsPlayedByPlayer1 == 3 && cardsPlayedByPlayer2 == 3)
        {
            InitData(true);
            firstTurn = state; //현재 케릭터의 턴 정보를 기록해두어 다음 라운드에선 현재 케릭터가 먼저 공격할 수 있도록합니다.
            StartCoroutine(ChangeGameState(GameState.CardSelect));
        }
        else
        {
            InitData(false);
            BeginNextTurn();
        }
    }

    void BeginNextTurn()
    {
        Debug.Log(state.ToString() + " 끝!");
        if (state == GameState.Player1Turn)
        {
            StartCoroutine(ChangeGameState(GameState.Player2Turn));
        }
        else if (state == GameState.Player2Turn)
        {
            StartCoroutine(ChangeGameState(GameState.Player1Turn));
        }
    }

    private void ChangeCharacterStateToMoveState()
    {
        Debug.Log("MOVE 카드 발동!");
        currentCharacter.characterStateMachine.ChangeState(currentCharacter.characterStateMachine.moveState);
    }

    private void ChangeCharacterStateToDefenseState()
    {
        Debug.Log("방어 카드 발동!");
        currentCharacter.characterStateMachine.ChangeState(currentCharacter.characterStateMachine.defenseState);
    }

    private void ChangeCharacterStateToBuffState()
    {
        Debug.Log("회복 카드 발동!");
        currentCharacter.characterStateMachine.ChangeState(currentCharacter.characterStateMachine.buffState);
    }

    private void ChangeCharacterStateToAttackState(AttackCard attackCard)
    {
        Debug.Log("공격 카드 발동!");
        AddStamina(attackCard.cardData.Stamina);
        currentCharacter.characterStateMachine.ChangeState(currentCharacter.characterStateMachine.attackState);
    }

    public void ApplyDamage(int damage)
    {
        Debug.Log("데미지 주기!");
        if (currentEnemy.IsDefenseCard)
        {
            damage -= currentCharacter.ReduceDamage;

            AddDamage(damage);
        }
        else AddDamage(damage);

        Debug.Log("데미지는 " + damage);
        currentEnemy.characterStateMachine.ChangeState(currentEnemy.characterStateMachine.takeDamageState);
    }

    public void AddDamage(int damage)
    {
        switch (state)
        {
            case GameState.Player1Turn:
                player1Damage += damage;
                break;
            case GameState.Player2Turn:
                player2Damage += damage;
                break;
            default:
                Debug.Log("Error, State Problem From Add Damage");
                break;
        }
    }

    public void AddStamina(int stamina)
    {
        switch (state)
        {
            case GameState.Player1Turn:
                player1Stamina += stamina;
                break;
            case GameState.Player2Turn:
                player2Stamina += stamina;
                break;
            default:
                Debug.Log("Error, State Problem From Add Stamina");
                break;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
}
