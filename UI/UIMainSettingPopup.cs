using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIMainSettingPopup : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
        quitButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickExitButton()
    {
        GameManager.Instance.matchView.SetActive(true);
        GameManager.Instance.cardSelectUI.SetActive(false);
        GameManager.Instance.conditionUI.SetActive(false);
        Destroy(GameManager.Instance.playerCharacter.gameObject);
        Destroy(GameManager.Instance.aiCharacter.gameObject);
        gameObject.SetActive(false);
    }

    private void OnClickQuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
