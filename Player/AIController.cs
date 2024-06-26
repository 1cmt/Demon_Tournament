using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public BaseCharacter aiCharacter;
    public BaseCharacter playerCharacter;
    public FieldController fieldController;

    public List<Card> moveCards = new List<Card>();
    public List<Card> attackCards = new List<Card>();
    public List<Card> defenseCards = new List<Card>();
    public List<Card> buffCards = new List<Card>();

    public int minStamina = 100;
    public int attackableDistance = 2;
    public int queueCount = 3;

    private void Awake()
    {
        aiCharacter = GetComponent<BaseCharacter>();
        fieldController = GameManager.Instance.fieldController;
        playerCharacter = GameManager.Instance.playerCharacter;
    }

    private void Start()
    {
        GetMinStamina();
        Init();
    }

    public void Init()
    {
        RefreshCard();
        Debug.Log("card Init");
        for (int i = 0; i < aiCharacter.cardList.Count; i++)
        {
            Card card = aiCharacter.cardList[i].GetComponent<Card>();
            switch (card.cardData.cardType)
            {
                case (CardType.Move):
                    moveCards.Add(card);
                    break;
                case (CardType.Attack):
                    attackCards.Add(card);
                    break;
                case (CardType.Defense):
                    defenseCards.Add(card);
                    break;
                case (CardType.Buff):
                    buffCards.Add(card);
                    break;
            }
        }
    }

    public void PickRandomCard()
    {
        RefreshCardList();
        int count = queueCount;
        int idx = 0;
        if (aiCharacter.stamina.curStamina < minStamina)
        {
            for (int i = 0; i < buffCards.Count; i++)
            {
                Card card = buffCards[i];
                card.cardData.IsUsed = true;
                aiCharacter.cardQueue.Enqueue(card);
                GameManager.Instance.utils.enemyCards[idx].gameObject.SetActive(true);
                GameManager.Instance.utils.enemyCards[idx].SetCardSprite(card);
                idx++;
            }
        }
        if (CheckAttackRange())
        {
            while (true)
            {

                Card card = attackCards[Random.Range(0, attackCards.Count)].GetComponent<Card>();
                if (card.cardData.IsUsed || card.GetCardType() != CardType.Attack) //사용한 카드면 다시뽑기
                    continue;
                if (!CheckStamina(card)) break;
                card.cardData.IsUsed = true;
                aiCharacter.cardQueue.Enqueue(card);
                GameManager.Instance.utils.enemyCards[idx].gameObject.SetActive(true);
                GameManager.Instance.utils.enemyCards[idx].SetCardSprite(card);
                idx++;
                break;
            }
        }
        else
        {
            while (true)
            {
                //움직임 카드 뽑기
                Card card = moveCards[Random.Range(0, moveCards.Count)].GetComponent<Card>();
                if (card.cardData.IsUsed || card.GetCardType() != CardType.Move) //사용한 카드면 다시뽑기
                    continue;
                card.cardData.IsUsed = true;
                aiCharacter.cardQueue.Enqueue(card);
                GameManager.Instance.utils.enemyCards[idx].gameObject.SetActive(true);
                GameManager.Instance.utils.enemyCards[idx].SetCardSprite(card);
                idx++;
                break;
            }
        }
        //완전 랜덤

        while (true)
        {
            if (count == idx)
                return;
            Card card = aiCharacter.cardList[Random.Range(0, aiCharacter.cardList.Count)].GetComponent<Card>();
            if (card.cardData.IsUsed || !CheckStamina(card)) //사용한 카드면 다시뽑기
                continue;
            card.cardData.IsUsed = true;
            aiCharacter.cardQueue.Enqueue(card);
            GameManager.Instance.utils.enemyCards[idx].gameObject.SetActive(true);
            GameManager.Instance.utils.enemyCards[idx].SetCardSprite(card);
            idx++;
        }

    }

    public void RefreshCard()
    {
        for (int i = 0; i < aiCharacter.cardList.Count; i++)
        {
            aiCharacter.cardList[i].GetComponent<Card>().cardData.IsUsed = false;
        }

        

    }
    public void RefreshCardList()
    {
        for (int i = 0; i < attackCards.Count; i++)
        {
            attackCards[i].cardData.IsUsed = false;
        }
        for (int i = 0; i < moveCards.Count; i++)
        {
            moveCards[i].cardData.IsUsed = false;
        }
        for (int i = 0; i < buffCards.Count; i++)
        {
            buffCards[i].cardData.IsUsed = false;
        }
        for (int i = 0; i < defenseCards.Count; i++)
        {
            defenseCards[i].cardData.IsUsed = false;
        }
    }


    //플레이어가 공격범위내에 있는지 확인
    public bool CheckAttackRange()
    {
        float distance = (playerCharacter.curGridPos - aiCharacter.curGridPos).magnitude;
        if (distance <= Mathf.Sqrt(attackableDistance))
            return true;
        return false;

        //Debug.Log(fieldController.GetGridPos(aiCharacter.curPosition));
    }

    //보유 카드중 가장 스태미나가 적게드는 것을 가져온다.
    public void GetMinStamina()
    {
        for (int i = 0; i < aiCharacter.cardList.Count; i++)
        {
            Card card = aiCharacter.cardList[i].GetComponent<Card>();
            if (card.GetCardType() == CardType.Attack) //Attack카드일경우에만 스태미나을 가져옴
            {
                minStamina = Mathf.Min(minStamina, card.cardData.Stamina);
            }
        }
    }

    public bool CheckStamina(Card card)
    {
        if (card.GetCardType() != CardType.Attack) return true;
        if (aiCharacter.stamina.curStamina < card.cardData.Stamina) return false;
        return true;
    }
}
