using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectView : MonoBehaviour
{
    [Header("UI View")]
    [SerializeField] private GameObject _titleView;

    [Header("Tween")]
    [SerializeField] private List<DOTweenAnimation> _tweens;

    [Header("Character Button")]
    [SerializeField] private List<Button> _buttons;

    [Header("Character Prefab")]
    [SerializeField] private List<BaseCharacter> _characters;

    private AudioSource _audioSource;
    private bool isOver= false;
    [SerializeField] private bool singleMode;

    private void Awake()
    {
        InitButton();
        AddTrigger();
    }

    private void OnEnable()
    {
        InitTween();
        singleMode = GameManager.Instance.singleMode;
    }

    private void InitButton()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            int idx = i;
            _buttons[i].onClick.AddListener(() => OnSelectCharacter(idx));
        }
    }

    private void AddTrigger()
    {
        for(int i= 0; i<_buttons.Count; i++)
        {
            // 버튼에 컴포넌트 추가 
            EventTrigger trigger = _buttons[i].gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = _buttons[i].gameObject.AddComponent<EventTrigger>();
            }

            // 새로운 객체 생성 
            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                // 마우스가 버튼 위로 올라갔을 때로 설정 
                eventID = EventTriggerType.PointerEnter
            };

            pointerEnterEntry.callback.AddListener((eventData) => {
                MousePointerOver((PointerEventData)eventData);
            Debug.Log(eventData.selectedObject);
            });
            trigger.triggers.Add(pointerEnterEntry);
        }
    }


    private void InitTween()
    {
        foreach (var tween in _tweens)
        {
            tween.DORestart();
        }
    }

    public void OnSelectCharacter(int playerIdx)
    {
        // 기존 데이터 초기화
        CharacterManager.Instance.aiCharacterDictionary.Clear();
        CharacterManager.Instance.aiCharacterIndexs.Clear();

        // CharacterManager에 플레이어 캐릭터 설정
        CharacterManager.Instance.playerCharacter = _characters[playerIdx];
        CharacterManager.Instance.playerCharacterIndex = playerIdx;

        // CharacterManager에 ai 캐릭터 설정

        int dictKey = 0;

        if (singleMode)
        {
            for (int i = 0; i < _characters.Count; i++)
            {
                if (i != playerIdx)
                {
                    CharacterManager.Instance.aiCharacterDictionary.Add(dictKey, _characters[i]);
                    //이코드는 단순 UI를 위한 인덱스 저장코드
                    CharacterManager.Instance.aiCharacterIndexs.Add(i);
                    dictKey++;
                }
            }

            GameManager.Instance.hasMovedScene = true;

            gameObject.SetActive(false);
            LoadingManager.Instance.LoadScene(1);
        }
        else
        {
            GameManager.Instance.hasMovedScene = true;
            NetworkManager.Instance.ConnectToPhoton();
            gameObject.SetActive(false);
            LoadingManager.Instance.LoadScene(2);
        }

    }

    public void OnClickReturnButton()
    {
        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
        gameObject.SetActive(false);
        _titleView.SetActive(true);
    }

    public void MousePointerOver(PointerEventData eventData)
    {
        SoundManager.Instance.PlayAudio(Sound.Effect, "firstSelect");
        Debug.Log("너 왜 소리가 나니?");
        Debug.Log(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //SoundManager.Instance.StopSFX();
    }
}
