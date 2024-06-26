using System.Collections.Generic;
using UnityEngine;

public class MainSceneUtils : MonoBehaviour
{
    [Header("UI")]
    public GameObject canvas;
    public GameObject cardSelectUI;
    public GameObject matchView;
    public GameObject conditionUI;
    public GameObject popupWin;
    public GameObject popupLose;
    public GameObject damageText;

    [Header("Player Card")]
    public List<SceneCard> playerCards;

    [Header("Enemy Card")]
    public List<SceneCard> enemyCards;

    private void Start()
    {
        GameManager.Instance.OnChangePlayerHP += ShowAIDamage;
        GameManager.Instance.OnChangeAIHP += ShowPlayerDamage;
    }

    // 플레이어가 입는 데미지
    private void ShowAIDamage()
    {
        GameObject text = Instantiate(damageText, canvas.transform);
        Vector3 offset = new Vector3 (0, 0.8f, 0);
        Vector3 playerPosition = Camera.main.WorldToScreenPoint(GameManager.Instance.playerCharacter.gameObject.transform.position + offset);
        text.transform.position = playerPosition;
        text.GetComponent<DamageText>().SetText(GameManager.Instance.aiDamage);

    }

    // AI가 입는 데미지
    private void ShowPlayerDamage()
    {
        GameObject text = Instantiate(damageText, canvas.transform);
        Vector3 offset = new Vector3(0, 0.8f, 0);
        Vector3 aiPosition = Camera.main.WorldToScreenPoint(GameManager.Instance.aiCharacter.gameObject.transform.position + offset);
        text.transform.position = aiPosition;
        text.GetComponent<DamageText>().SetText(GameManager.Instance.playerDamage);
    }
}
