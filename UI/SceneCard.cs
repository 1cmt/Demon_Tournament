using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneCard : MonoBehaviour
{
    public RectTransform target;
    public Sprite front;
    public Sprite back;
    public Image cardImage;

    private void Start()
    {
        cardImage = GetComponent<Image>();
    }

    // 카드 이미지 세팅
    public void SetCardSprite(Card card)
    {
        front = card.cardData.CardSprite;
        back = card.cardData.CardBackSprite;

        cardImage.sprite = back;
    }

    // 카드 사용할 때 가져다 쓰기
    public void UseCard()
    {
        StartCoroutine(RotateCard());
    }

    // 카드 뒤집히는 애니메이션
    private IEnumerator RotateCard()
    {
        var sequenece = DOTween.Sequence();
        sequenece.Append(target.DORotate(target.eulerAngles + new Vector3(0, 90, 0), 0.25f));
        sequenece.AppendCallback(() => { cardImage.sprite = front; });
        sequenece.Append(target.DORotate(target.eulerAngles + new Vector3(0, 00, 0), 0.25f));

        yield return new WaitForSeconds(1.5f);

        gameObject.SetActive(false);
    }
}
