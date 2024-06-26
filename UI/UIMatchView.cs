using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMatchView : MonoBehaviourPunCallbacks
{
    public static UIMatchView Instance;

    [Serializable]
    public class MatchViewCharacter
    {
        [Header("Character Icons & Name")]
        public List<GameObject> icons;
        public GameObject locked;
        public TextMeshProUGUI name;
    }

    [Header("UI")]
    [SerializeField] private GameObject _cardView;
    [SerializeField] private GameObject _uiCondition;
    [SerializeField] private Button _changeButton;
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private List<Sprite> _backgrounds;

    [SerializeField] private GameObject _dialogView;

    [Header("Player")]
    public MatchViewCharacter playerCharacter;

    [Header("Enemy")]
    public List<MatchViewCharacter> enemyCharacters;

    [Header("Tween")]
    [SerializeField] private DOTweenAnimation _buttonGroup;

    private void Awake()
    {
        Instance = this;
        InitCharacter();
    }

    private void Start()
    {
        SoundManager.Instance.PlayAudio(Sound.Effect, "SelectAfter");
    }

    private void OnEnable()
    {
        _buttonGroup.DORestart();
        if (GameManager.Instance.singleMode)
        {
            InitMatchView();
        }
    }

    // 초기 캐릭터 아이콘, 이름 세팅
    private void InitCharacter()
    {
        // 플레이어
        playerCharacter.icons[CharacterManager.Instance.playerCharacterIndex].SetActive(true);
        playerCharacter.name.text = CharacterManager.Instance.playerCharacter.characterData.Name;

        // AI
        if (GameManager.Instance.singleMode)
        {
            int enemyIdx = 0;
            int idx = 0;
            foreach (var enemyIndex in CharacterManager.Instance.aiCharacterDictionary.Keys)
            {
                if (enemyIndex == CharacterManager.Instance.playerCharacterIndex) { idx++; }
                enemyCharacters[enemyIdx].icons[idx].SetActive(true);
                enemyCharacters[enemyIdx].name.text = CharacterManager.Instance.aiCharacterDictionary[enemyIdx].characterData.Name;
                enemyIdx++;
                idx++;
            }
        }
        else
        {
            // 멀티플레이어 모드에서는 Photon 네트워크를 통해 상대방 캐릭터 정보를 업데이트
             //UpdateEnemyCharacterUI(CharacterManager.Instance.enemyCharacterIndex);
        }
    }

    private void InitMatchView()
    {
        // 첫 전투가 아니면 캐릭터 선택 버튼 비활성화
        if (GameManager.Instance.hasFighted)
        {
            _changeButton.interactable = false;
        }

        // 현재 스테이지 잠금 해제
        enemyCharacters[GameManager.Instance.stageNum].locked.SetActive(false);
    }

    public void OnClickChangeButton()
    {
        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
        LoadingManager.Instance.LoadScene(0);
    }

    public void OnClickFightButton()
    {
        //Fight 버튼 누르면 플레이어는 확정이기에 게임매니저에 등록
        GameManager.Instance.playerCharacter = CharacterManager.Instance.playerCharacter;

        if (!GameManager.Instance.hasFighted)
        {
            GameManager.Instance.hasFighted = true;
        }

        GameManager.Instance.StartStage();

        // 랜덤으로 배경 교체
        int rand = UnityEngine.Random.Range(0, _backgrounds.Count);
        _background.sprite = _backgrounds[rand];

        _cardView.SetActive(true);
        _uiCondition.SetActive(true);
        _dialogView.SetActive(true);
        gameObject.SetActive(false);
        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
    }
}
    