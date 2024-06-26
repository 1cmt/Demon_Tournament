using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [Header("Controller")]
    public static JoinController joinController;
    public static LoginController logininController;

    [Header("ObjectController")]
    [SerializeField] private GameObject[] menu;
    [SerializeField] private Button[] uiSetBtn;
    [SerializeField] private Button[] uiCheckBtn;
    [SerializeField] private Button[] uiCloseBtn;

    [Header("Set Active Objects")]
    [SerializeField] private GameObject uiPromptTxt;

    private bool loginMenuChk = false;

    private void Start()
    {
        joinController = JoinController.Instance;
        logininController = LoginController.Instance;

        // 로그인 버튼 이벤트
        for (int i = 0; i < uiCloseBtn.Length; i++)
        {
            int index = i;
            uiSetBtn[i].onClick.AddListener(() => OnSetActiveMenu(index));
            uiCheckBtn[i].onClick.AddListener(() => OnCheckActiveMenu(index));
            uiCloseBtn[i].onClick.AddListener(() => OnCloseButtonClick(index));
        }
    }

    // 스페이스바 눌렀을 때 로그인 화면 나오는 조건문
    void Update()
    {
        if (menu[0] != null && Input.GetKeyDown(KeyCode.Space)) 
        {
            if (!loginMenuChk)
            {
                uiPromptTxt.SetActive(false);
                menu[0].SetActive(true);
                loginMenuChk = true;
            }
            else
            {
                menu[0].SetActive(false);
                loginMenuChk = false;
            }
        }
    }

    public void OnCloseButtonClick(int index)
    {
        switch (index)
        {
            case 0:
                menu[1].SetActive(false);
                SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
                break;
            case 1:
                menu[2].SetActive(false);
                SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
                break;
        }
    }

    public void OnSetActiveMenu(int index)
    {
        switch (index)
        {
            case 0:
                menu[1].SetActive(true);
                break;
            case 1:

                SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
                menu[2].SetActive(true);
                break;
        }
    }

    public void OnCheckActiveMenu(int index)
    {
        switch (index)
        {
            case 0:
                logininController.OnLoginBtnClick();
                break;
            case 1:
                joinController.OnJoinBtnClick();
                break;
        }
    }
}
