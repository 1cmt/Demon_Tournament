using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UIResultPopup : MonoBehaviour
{
    [Header("Tween")]
    [SerializeField] private List<DOTweenAnimation> _tweens;

    private void OnEnable()
    {
        InitTween();
    }

    private void InitTween()
    {
        foreach (var tween in _tweens)
        {
            tween.DORestart();
        }
    }

    public void OnClickNextButton()
    {
        GameManager.Instance.matchView.SetActive(true);
        GameManager.Instance.cardSelectUI.SetActive(false);
        GameManager.Instance.conditionUI.SetActive(false);
        gameObject.SetActive(false);
    }
}
