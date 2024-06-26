using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //게임매니저에 너무 많은 역이 있다판단해 상태머신에 많은 코드들을 이동시켰습니다.

    public override void Awake()
    {
        base.Awake();
    }

    public FieldController fieldController;
    public float Xoffset = -1.0f;
    public float _Xoffset = 1.0f;
    public float Yoffset = 0.3f;

    public BaseCharacter currentCharacter;
    public BaseCharacter currentEnemy;

    public enum GameState
    {
        CardSelect, //카드 선택
        PlayerTurn,
        AITurn,
        PlayerWin,
        AIWin
    }

    public bool singleMode = false;
    public GameState state;
    public GameState firstTurn; //처음 시작할 턴, ai먼저 하도록 한다.

    public BaseCharacter playerCharacter;
    public BaseCharacter aiCharacter;

    public int stageNum;

    public int playerDamage;
    public int aiDamage;
    public int playerStamina;
    public int aiStamina;
    public int cardsPlayedByPlayer;
    public int cardsPlayedByAI;

    #region 멀티
    public int player1Damage;
    public int player2Damage;
    public int player1Stamina;
    public int player2Stamina;
    public int cardsPlayedByPlayer1;
    public int cardsPlayedByPlayer2;
    #endregion

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

    public event Action OnChangePlayerHP;
    public event Action OnChangeAIHP;
    public event Action OnChangeSP;
    public event Action OnShieldBroken;

    public MainSceneUtils utils;

    void Start()
    {
        firstTurn = GameState.AITurn; //처음엔 ai부터 (랜덤값으로 바꿀 수도 있습니다)
        hasFighted = false;
    }

    private IEnumerator ChangeGameState(GameState state)
    {
        yield return null; //호출한 함수가 종료될 수 있게
        Debug.Log(state.ToString() + "시작");

        this.state = state;
        switch (state)
        {
            case GameState.CardSelect:
                //카드선택 상태에선 카드선택 UI가 보이도록 합니다.
                ShowCardSelectUI();
                break;
            case GameState.PlayerTurn:
                currentCharacter = playerCharacter;
                currentEnemy = aiCharacter;
                cardsPlayedByPlayer++;
                StartCoroutine(PlayCard());
                break;
            case GameState.AITurn:
                currentCharacter = aiCharacter;
                currentEnemy = playerCharacter;
                cardsPlayedByAI++;
                StartCoroutine(PlayCard());
                break;
            case GameState.PlayerWin:
                SetPlayerWin();
                SoundManager.Instance.StopBGM();
                break;
            case GameState.AIWin:
                SetAIWin();
                SoundManager.Instance.StopBGM();
                break;
            default:
                break;
        }
    }

    void SetInitialPositions()
    {
        // 초기 위치 설정
        Debug.Log("초기 위치 설정");

        if (singleMode)
        {
            fieldController.MovePlayerToCell(playerCharacter.transform, fieldController.playerGridPosition, Xoffset, Yoffset);
            fieldController.MovePlayerToCell(aiCharacter.transform, fieldController.aiGridPosition, _Xoffset, Yoffset);
            aiCharacter.transform.rotation = Quaternion.Euler(0, 180f, 0);

            playerCharacter.curGridPos = fieldController.playerGridPosition;
            aiCharacter.curGridPos = fieldController.aiGridPosition;
        }
        else
        {
            #region 멀티
            Transform player1 = CharacterManager.Instance.playerCharacter.gameObject.transform;
            Transform player2 = CharacterManager.Instance.player2Character.gameObject.transform;

            fieldController.MovePlayerToCell(player1, fieldController.playerGridPosition, Xoffset, Yoffset);
            fieldController.MovePlayerToCell(player2, fieldController.aiGridPosition, _Xoffset, Yoffset);
            player2.rotation = Quaternion.Euler(0, 180f, 0);

            CharacterManager.Instance.playerCharacter.curGridPos = fieldController.playerGridPosition;
            CharacterManager.Instance.player2Character.curGridPos = fieldController.aiGridPosition;
            #endregion
        }


    }

    void InitData(bool isRoundEnd)
    {
        if (singleMode)
        {
            playerDamage = 0;
            aiDamage = 0;
            playerStamina = 0;
            aiStamina = 0;
            if (isRoundEnd)
            {
                cardsPlayedByPlayer = 0;
                cardsPlayedByAI = 0;
            }
        }
        else
        {
            #region 멀티
            player1Damage = 0;
            player2Damage = 0;
            player1Stamina = 0;
            player2Stamina = 0;
            if (isRoundEnd)
            {
                cardsPlayedByPlayer1 = 0;
                cardsPlayedByPlayer2 = 0;
            }
            #endregion
        }
    }

    public Vector3 GetWorldPos(Vector2 girdPos, bool isLeft)
    {
        if (isLeft) return fieldController.GetWorldPos(girdPos, Xoffset, Yoffset);
        else return fieldController.GetWorldPos(girdPos, _Xoffset, Yoffset);
    }

    public Vector3 GetWorldPos(Vector2 girdPos, bool isLeft, float xOffset)
    {
        if (isLeft) return fieldController.GetWorldPos(girdPos, -xOffset, Yoffset);
        else return fieldController.GetWorldPos(girdPos, xOffset, Yoffset);
    }

    public void CreatePrefab(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, prefab.transform.rotation);
    }

    public void CreatePrefab(GameObject prefab, Vector3 position, float delay)
    {
        StartCoroutine(CreatePrefabWithDelay(prefab, position, delay));
    }

    private IEnumerator CreatePrefabWithDelay(GameObject prefab, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(prefab, position, prefab.transform.rotation);
    }

    //게임이 시작되면 해당 함수를 수행하도록 하면 될거 같습니다.
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

        if (singleMode)
        {
            GameObject playerObj = Instantiate(CharacterManager.Instance.playerCharacter.gameObject);
            GameObject aiObj = Instantiate(CharacterManager.Instance.aiCharacterDictionary[stageNum].gameObject);

            playerCharacter = playerObj.GetComponent<BaseCharacter>();
            aiCharacter = aiObj.GetComponent<BaseCharacter>();

            playerCharacter.IsLeft = true;
            aiCharacter.IsLeft = false;

            aiObj.AddComponent<AIController>();
        }
        else
        {
            #region 멀티
            // 플레이어 1과 플레이어 2의 프리팹을 Photon PUN 2로 인스턴스화
            string player1PrefabName = CharacterManager.Instance.playerCharacter.gameObject.name;
            string player2PrefabName = CharacterManager.Instance.player2Character.gameObject.name;

            GameObject p1 = PhotonNetwork.Instantiate($"{player1PrefabName}", Vector3.zero, Quaternion.identity);
            GameObject p2 = PhotonNetwork.Instantiate($"{player2PrefabName}", Vector3.zero, Quaternion.identity);

            CharacterManager.Instance.playerCharacter = p1.GetComponent<BaseCharacter>();
            CharacterManager.Instance.player2Character = p2.GetComponent<BaseCharacter>();

            // 마스터 클라이언트 여부에 따라 초기 위치와 방향 설정
            if (PhotonNetwork.IsMasterClient)
            {
                CharacterManager.Instance.playerCharacter.IsLeft = true;
                CharacterManager.Instance.player2Character.IsLeft = false;
            }
            else
            {
                CharacterManager.Instance.playerCharacter.IsLeft = false;
                CharacterManager.Instance.player2Character.IsLeft = true;
            }
            #endregion
        }

        InitData(true); //데이터 초기화
        SetInitialPositions(); //플레이어 위치 초기화

        //설정 초기화 후 카드 선택 상태를 수행합니다.
        StartCoroutine(ChangeGameState(GameState.CardSelect));
    }

    private void ShowCardSelectUI()
    {
        cardSelectUI.SetActive(true);

        // cardSelectUI 초기화
        cardSelectUI.GetComponent<UICardView>().OnClickClearButton();
    }

    private void SetPlayerWin()
    {
        Time.timeScale = 0;
        isPaused = true;
        stageNum++;
        Destroy(playerCharacter.gameObject);
        Destroy(aiCharacter.gameObject);
        popupWin.SetActive(true);
    }

    private void SetAIWin()
    {
        Time.timeScale = 0;
        isPaused = true;
        Destroy(playerCharacter.gameObject);
        Destroy(aiCharacter.gameObject);
        popupLose.SetActive(true);
    }

    //카드 선택 UI에서 Continue를 클릭시 해당함수를 수행하도록 하면 됩니다.
    public void Continue()
    {
        //firstTurn엔 먼저 시작할 케릭터의 턴이 담겨있습니다.
        StartCoroutine(ChangeGameState(firstTurn));
    }

    private IEnumerator PlayCard()
    {
        yield return null; //호출한 코루틴 함수는 종료될 수 있게

        Debug.Log("카드 뽑기");
        Card card = currentCharacter.cardQueue.Dequeue().GetComponent<Card>();
        currentCharacter.currentCard = card;
        float time = 0;

        // 카드 애니메이션 실행
        if (currentCharacter == playerCharacter)
        {
            utils.playerCards[cardsPlayedByPlayer - 1].UseCard();
        }
        else
        {
            utils.enemyCards[cardsPlayedByAI - 1].UseCard();
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
        Debug.Log("플레이어 턴 수 : " + cardsPlayedByPlayer + " ai 턴 수 : " + cardsPlayedByAI);
        if (cardsPlayedByPlayer == cardsPlayedByAI) StartCoroutine(EndBothTurn()); //둘이 카드 꺼낸 횟수가 같으면 서로의 턴이 종료된 것입니다.

        else BeginNextTurn(); //횟수가 다르면 상대 턴이 남아 있는 것임
    }

    void BeginNextTurn()
    {
        if (state == GameState.PlayerTurn)
        {
            StartCoroutine(ChangeGameState(GameState.AITurn));
        }
        else if (state == GameState.AITurn)
        {

            StartCoroutine(ChangeGameState(GameState.PlayerTurn));
        }
    }

    private IEnumerator EndBothTurn()
    {
        Debug.Log("둘다 턴 끝!");
        //턴이 끝났음을 알려 턴 동안 지속되는 게임오브젝트가 제거될 수 있도록
        OnShieldBroken?.Invoke();

        aiCharacter.health.TakeDamage(playerDamage);
        playerCharacter.health.TakeDamage(aiDamage);

        playerCharacter.stamina.UseStamina(playerStamina);
        aiCharacter.stamina.UseStamina(aiStamina);

        Debug.Log("케릭터 스태미나 : " + playerCharacter.stamina.curStamina + " ai스태미나 : " + aiCharacter.stamina.curStamina);

        playerCharacter.IsDefenseCard = false;
        aiCharacter.IsDefenseCard = false;

        // 컨디션 변경 이벤트 호출
        OnChangeSP?.Invoke();
        if (aiDamage != 0) OnChangePlayerHP?.Invoke();
        if (playerDamage != 0) OnChangeAIHP?.Invoke();

        if (playerDamage != 0 || aiDamage != 0) yield return new WaitForSeconds(updateHpBarWait);

        Debug.Log("Player Health: " + playerCharacter.health.curHealth + ", AI Health: " + aiCharacter.health.curHealth);

        // 한쪽의 체력이 0이하면 게임 종료
        if (aiCharacter.health.curHealth <= 0) StartCoroutine(ChangeGameState(GameState.PlayerWin));
        else if (playerCharacter.health.curHealth <= 0) StartCoroutine(ChangeGameState(GameState.AIWin));

        // 3개 카드 주고 받았으면 라운드가 끝난 것이다. 아니면 다음턴 또 시작
        if (cardsPlayedByPlayer == 3 && cardsPlayedByAI == 3)
        {
            InitData(true);
            firstTurn = state; //현재 케릭터의 턴 정보를 기록해두어 다음 라운드에선 현재 케릭터가 먼저 공격할 수 있도록합니다.
            SoundManager.Instance.StopBGM();
            //라운드가 끝났으니 카드 선택 상태로 갑니다.
            StartCoroutine(ChangeGameState(GameState.CardSelect));

         
        }
        else
        {
            InitData(false);
            BeginNewTurn();
        }
    }

    void BeginNewTurn()
    {
        Debug.Log(state.ToString() + "끝!");
        Card nextPlayerCard = playerCharacter.cardQueue.Peek();
        Card nextAICard = aiCharacter.cardQueue.Peek();

        //만일 다음턴이 ai인 상황에 ai는 공격카드 내 다음카드가 방어 카드라면 ai가 공격한다음에 방어카드를 쓰기에 방어가 안된다.
        //고로 이런상황을 막기위해 지금 내턴이라면 다음 턴에 내가 방어카드, 적이 공격카드면 턴을 적에게 넘기는게 아니라 한번더 턴을 쓴다.
        switch (state)
        {
            case GameState.PlayerTurn:
                if (nextPlayerCard is DefenseCard && nextAICard is AttackCard) StartCoroutine(ChangeGameState(GameState.PlayerTurn));
                else StartCoroutine(ChangeGameState(GameState.AITurn));
                return;
            case GameState.AITurn:
                if (nextAICard is DefenseCard && nextPlayerCard is AttackCard) StartCoroutine(ChangeGameState(GameState.AITurn));
                else StartCoroutine(ChangeGameState(GameState.PlayerTurn));
                return;
            default:
                Debug.Log("Error");
                return;
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

    public void ApplyDamage(int damage, float delay)
    {
        StartCoroutine(ApplyDamageWithDelay(damage, delay));
    }

    private IEnumerator ApplyDamageWithDelay(int damage, float delay)
    {
        Debug.Log("데미지 주기!");
        if (currentEnemy.IsDefenseCard)
        {
            damage -= currentCharacter.ReduceDamage;

            AddDamage(damage);
        }
        else AddDamage(damage);

        Debug.Log("데미지는 " + damage);
        
        //TODO: 적이 피해 받았을때 피해데미지도 표시하면 좋을듯
        yield return new WaitForSeconds(delay);
        currentEnemy.characterStateMachine.ChangeState(currentEnemy.characterStateMachine.takeDamageState);
    }

    public void AddDamage(int damage)
    {
        switch (state)
        {
            case GameState.PlayerTurn:
                playerDamage += damage;
                break;
            case GameState.AITurn:
                aiDamage += damage;
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
            case GameState.PlayerTurn:
                playerStamina += stamina;
                break;
            case GameState.AITurn:
                aiStamina += stamina;
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
