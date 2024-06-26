using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICondition : MonoBehaviour
{
    [Header("Player left")]
    [SerializeField] private Image _leftPlayerIcon;
    [SerializeField] private Slider _leftPlayerHPBar;
    [SerializeField] private Slider _leftPlayerSPBar;
    [SerializeField] private TextMeshProUGUI _leftPlayerHPText;

    [Header("Player Right")]
    [SerializeField] private Image _rightPlayerIcon;
    [SerializeField] private Slider _rightPlayerHPBar;
    [SerializeField] private Slider _rightPlayerSPBar;
    [SerializeField] private TextMeshProUGUI _rightPlayerHPText;

    [Header("Icons")]
    [SerializeField] private List<Sprite> _icons;

    private void OnEnable ()
    {
        InitIcon();
        InitCondition();
        SetConditionEvent();
    }

    #region Condition 초기화
    private void InitIcon()
    {
        _leftPlayerIcon.sprite = _icons[CharacterManager.Instance.playerCharacterIndex];
        _rightPlayerIcon.sprite = _icons[CharacterManager.Instance.GetCurrentAIIndex()];
    }

    private void InitCondition()
    {
        UpdateLeftPlayerHP();
        UpdateLeftPlayerSP();
        UpdateRightPlayerHP();
        UpdateRightPlayerSP();
    }
    #endregion

    #region Condition 업데이트
    private void SetConditionEvent()
    {
        GameManager.Instance.OnChangePlayerHP += UpdateLeftPlayerHP;
        GameManager.Instance.OnChangeSP += UpdateLeftPlayerSP;

        GameManager.Instance.OnChangeAIHP += UpdateRightPlayerHP;
        GameManager.Instance.OnChangeSP += UpdateRightPlayerSP;
    }

    private void UpdateLeftPlayerHP()
    {
        int leftPlayerCurHP = GameManager.Instance.playerCharacter.health.curHealth;
        int leftPlayerMaxHP = GameManager.Instance.playerCharacter.health.maxHealth;
        Debug.Log(leftPlayerCurHP + " " + leftPlayerMaxHP);
        _leftPlayerHPBar.value = (float)leftPlayerCurHP / leftPlayerMaxHP;
        _leftPlayerHPText.text = $"{leftPlayerCurHP}/{leftPlayerMaxHP}";
    }

    private void UpdateLeftPlayerSP()
    {
        int leftPlayerCurSP = GameManager.Instance.playerCharacter.stamina.curStamina;
        int leftPlayerMaxSP = GameManager.Instance.playerCharacter.stamina.maxStamina;
        Debug.Log(leftPlayerCurSP + " " + leftPlayerMaxSP);

        _leftPlayerSPBar.value = (float)leftPlayerCurSP / leftPlayerMaxSP;
    }

    private void UpdateRightPlayerHP()
    {
        int rightPlayerCurHP = GameManager.Instance.aiCharacter.health.curHealth;
        int rightPlayerMaxHP = GameManager.Instance.aiCharacter.health.maxHealth;

        _rightPlayerHPBar.value = (float)rightPlayerCurHP / rightPlayerMaxHP;
        _rightPlayerHPText.text = $"{rightPlayerCurHP}/{rightPlayerMaxHP}";
    }

    private void UpdateRightPlayerSP()
    {
        int rightPlayerCurSP = GameManager.Instance.aiCharacter.stamina.curStamina;
        int rightPlayerMaxSP = GameManager.Instance.aiCharacter.stamina.maxStamina;

        _rightPlayerSPBar.value = (float)rightPlayerCurSP / rightPlayerMaxSP;
    }
    #endregion
}
