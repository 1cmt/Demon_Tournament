using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    public BaseCharacter playerCharacter; // 플레이어 캐릭터 오브젝트
    public BaseCharacter player2Character; // 플레이어 캐릭터 오브젝트
    public BaseCharacter aiCharacter; // AI 캐릭터 오브젝트
    public Dictionary<int, BaseCharacter> aiCharacterDictionary = new Dictionary<int, BaseCharacter>();
    public List<int> aiCharacterIndexs = new List<int>(); // 단순 UI만을 위한 인덱스 (로직에 영향 안 줌)
    public List<BaseCharacter> _characters; // 추가: 모든 캐릭터 리스트

    public int playerCharacterIndex;

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        // 인트로 씬에서 바로 호출됨으로 참조 오류 발생
        // InitializeCharacter(playerCharacter);
        // InitializeCharacter(aiCharacter);
    }

    void Update()
    {

    }

    void InitializeCharacter(BaseCharacter character)
    {
        // BaseCharacter 클래스의 characterData에 ScriptableObject 데이터 할당
        CharacterSO data = playerCharacter.characterData;

        // 플레이어가 이미 선택한 캐릭터인 경우 AI 캐릭터 데이터에서 IsPlayerSelected를 true로 설정
        if (data.IsPlayerSelected)
        {
            data.IsPlayerSelected = true; // AI가 선택할 수 없도록 설정
        }
        else
        {
            // 플레이어가 선택하지 않은 캐릭터라면 AI가 선택 가능
            data.IsPlayerSelected = false;
        }
    }

    public int GetCurrentAIIndex()
    {
        return aiCharacterIndexs[GameManager.Instance.stageNum];
    }
}
