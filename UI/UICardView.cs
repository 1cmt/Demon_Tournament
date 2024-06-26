using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardView : MonoBehaviour
{
    [Serializable]
    public class UICell
    {
        public List<RectTransform> cell;
    }

    [Header("Button UI")]
    [SerializeField] private GameObject _sceneCardGroup;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _clearButton;

    [Header("Card")]
    [SerializeField] private List<GameObject> _tempCardList;
    [SerializeField] private List<GameObject> _cardPositions;
    [SerializeField] private List<GameObject> _selectedCards;
    private readonly int _maxSelectNum = 3;
    private int _curSelectNum = 0;

    [Header("Character Position")]
    [SerializeField] private List<UICell> _uiGrid;
    [SerializeField] private RectTransform _playerPos;
    [SerializeField] private RectTransform _enemyPos;
    private readonly float _posOffset = 30f;

    [Header("Tween")]
    [SerializeField] private List<DOTweenAnimation> _tweens;

    private void Start()
    {
        InitCard();
        SoundManager.Instance.StopBGM();
    }

    private void Update()
    {
        if (_curSelectNum != _maxSelectNum)
        {
            _continueButton.interactable = false;
        }
        else
        {
            _continueButton.interactable = true;
        }
    }

    private void OnEnable()
    {
        InitTween();
        UpdateCharacterPos();
        UpdateSelectableCard();
    }

    private void InitTween()
    {
        foreach (var tween in _tweens)
        {
            tween.DORestart();
        }
    }

    private void InitCard()
    {
        for (int i = 0; i < _cardPositions.Count; i++)
        {
            // 카드 생성
            GameObject card = Instantiate(GameManager.Instance.playerCharacter.cardList[i], _cardPositions[i].transform);

            // 임시 리스트에 저장
            _tempCardList.Add(card);

            // 버튼 이벤트 등록
            if (card.TryGetComponent(out Button btn))
            {
                btn.onClick.AddListener(() => Select(card));
            }
        }
    }

    private void UpdateCharacterPos()
    {
        if (GameManager.Instance.singleMode)
        {
            Vector2 curPlayerPosition = GameManager.Instance.playerCharacter.curGridPos;
            Vector2 curEnemyPosition = GameManager.Instance.aiCharacter.curGridPos;

            _playerPos.position = _uiGrid[(int)curPlayerPosition.y].cell[(int)curPlayerPosition.x].position + Vector3.left * _posOffset;
            _enemyPos.position = _uiGrid[(int)curEnemyPosition.y].cell[(int)curEnemyPosition.x].position + Vector3.right * _posOffset;
        }
        else
        {
            Vector2 curPlayerPosition = GameManager.Instance.playerCharacter.curGridPos;
            Vector2 curEnemyPosition = GameManager.Instance.aiCharacter.curGridPos;

            _playerPos.position = _uiGrid[(int)curPlayerPosition.y].cell[(int)curPlayerPosition.x].position + Vector3.left * _posOffset;
            _enemyPos.position = _uiGrid[(int)curEnemyPosition.y].cell[(int)curEnemyPosition.x].position + Vector3.right * _posOffset;
        }
    }

    private void UpdateSelectableCard()
    {
        // 플레이어 스태미나에 따른 선택 제한
        for (int i = 0; i < _tempCardList.Count; i++)
        {
            Card card = _tempCardList[i].GetComponent<Card>();
            if (card.cardData.cardType == CardType.Buff) return;

            Button btn = _tempCardList[i].GetComponent<Button>();

            int playerStamina = GameManager.Instance.playerCharacter.stamina.curStamina;
            int cardStamina = card.cardData.Stamina;

            btn.interactable = playerStamina >= cardStamina;
        }
    }

    public void Select(GameObject cardObj)
    {
        // 카드 선택 횟수 제한 : 3회
        if (_curSelectNum == _maxSelectNum) return;

        // 플레이어 큐에 추가
        Card card = cardObj.GetComponent<Card>();
        GameManager.Instance.playerCharacter.cardQueue.Enqueue(card);

        // 하단 선택한 카드 목록에 추가
        _selectedCards[_curSelectNum].GetComponent<Image>().sprite = card.cardImage.sprite;
        _selectedCards[_curSelectNum].SetActive(true);

        // 씬의 카드 그룹 세팅
        GameManager.Instance.utils.playerCards[_curSelectNum].gameObject.SetActive(true);
        GameManager.Instance.utils.playerCards[_curSelectNum].SetCardSprite(card);

        cardObj.SetActive(false);

        _curSelectNum++;
        SoundManager.Instance.PlayAudio(Sound.Effect, "ThreeCardSelect");
    }

    public void OnClickContinueButton()
    {

        SoundManager.Instance.PlayAudio(Sound.Effect, "InGameFirst");

        SoundManager.Instance.PlayAudio(Sound.Bgm, "BGM5");
        Debug.Log(GameManager.Instance.aiCharacter.name);
        GameManager.Instance.aiCharacter.gameObject.GetComponent<AIController>().PickRandomCard();

        gameObject.SetActive(false);
        GameManager.Instance.Continue();
    }

    // 버튼 이벤트인데 한 라운드 끝나고 UI 초기화로 사용가능
    public void OnClickClearButton()
    {
        // 플레이어 큐 초기화
        if (GameManager.Instance.singleMode)
        {
            GameManager.Instance.playerCharacter.cardQueue.Clear();
        }
        else
        {
            GameManagerM.Instance.player1Character.cardQueue.Clear();
            GameManagerM.Instance.player2Character.cardQueue.Clear();
        }
        

        // 하단 선택한 카드 목록 비활성화
        for (int i = 0; i < _selectedCards.Count; i++)
        {
            _selectedCards[i].SetActive(false);
        }

        // 상단에 선택해서 비활성화된 카드 다시 활성화
        for (int i = 0; i < _tempCardList.Count; i++)
        {
            if (!_tempCardList[i].activeInHierarchy)
            {
                _tempCardList[i].SetActive(true);
            }
        }

        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
        _curSelectNum = 0;

        UpdateCharacterPos();
    }
}