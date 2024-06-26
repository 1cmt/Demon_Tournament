using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public Image cardImage;

    public CardType GetCardType()
    {
        return cardData.cardType;
    }

    protected virtual void Awake()
    {
        cardImage = GetComponent<Image>();

        cardImage.sprite = cardData.CardSprite;
    }
}