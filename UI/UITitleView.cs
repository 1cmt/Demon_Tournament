using DG.Tweening;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITitleView : MonoBehaviour
{
    [Header("UI View")]
    [SerializeField] private GameObject _selectView;
    [SerializeField] private GameObject _popupLogin;
    [SerializeField] private GameObject _popupRoomList;

    [Header("Tween")]
    [SerializeField] private List<DOTweenAnimation> _tweens;

    [Header("Set Active Objects")]
    [SerializeField] private TextMeshProUGUI alertMessage;

    private void Start()
    {
        SoundManager.Instance.PlayAudio(Sound.Bgm, "BGM");
    }

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

    public void OnClickPlayButton()
    {
        GameManager.Instance.singleMode = true;
        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
        gameObject.SetActive(false);
        _selectView.SetActive(true);
    }

    public void OnClickMultiButton()
    {
        GameManager.Instance.singleMode = false;
        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
        gameObject.SetActive(false);
        _popupRoomList.SetActive(true);
        NetworkManager.Instance.ConnectToPhoton();
    }


    public void OnClickExitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickLogoutButton()
    {
        PlayerManager.Instance.Logout();
        SetAlertMessage("로그아웃 되었습니다.");
        _popupLogin.SetActive(true);
    }

    private void SetAlertMessage(string str)
    {
        alertMessage.text = str;
        alertMessage.gameObject.SetActive(true);
    }
}
