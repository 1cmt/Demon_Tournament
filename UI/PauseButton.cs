using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject _popupSetting;
    private Button _pauseButton;

    private void Awake()
    {
        _pauseButton = GetComponent<Button>();
    }

    private void Start()
    {
        _pauseButton.onClick.AddListener(OnClickPauseButton);
    }

    private void OnClickPauseButton()
    {
        GameManager.Instance.TogglePause();

        if (GameManager.Instance.isPaused)
        {
            _popupSetting.SetActive(true);
        }
        else
        {
            _popupSetting.SetActive(false);
        }
    }
}
