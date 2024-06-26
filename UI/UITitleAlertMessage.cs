using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class UITitleAlertMessage : MonoBehaviour
{
    private float _delayTime = 1f;

    private DOTweenAnimation _tween;
    private TextMeshProUGUI _TMP;

    private void Awake()
    {
        _tween = GetComponent<DOTweenAnimation>();
        _TMP = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _tween.DORestart();
    }

    public void OnTweenFinished()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(WaitingForTween());
        }
    }

    private IEnumerator WaitingForTween()
    {
        _TMP.DOColor(Color.clear, _delayTime);

        yield return new WaitForSeconds(_delayTime);

        gameObject.SetActive(false);
    }
}
