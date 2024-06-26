using System.Collections;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class LoginController : MonoBehaviour
{
    public static LoginController Instance;

    [Header("Field")]
    [SerializeField] private TMP_InputField idField;
    [SerializeField] private TMP_InputField pwField;

    [Header("Set Active Objects")]
    [SerializeField] private GameObject popupLogin;
    [SerializeField] private GameObject titleButtons;
    [SerializeField] private GameObject settingButton;
    [SerializeField] private TextMeshProUGUI alertMessage;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    private void Awake()
    {
        Instance = this;
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.GetInstance("https://demontournament-32fb6-default-rtdb.firebaseio.com/").RootReference;
    }

    public void OnLoginBtnClick()
    {
        string email = idField.text.Trim() + "@demon.com";
        string password = pwField.text;

        StartCoroutine(LoginAsync(email, password));

        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "로그인 실패";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "유효하지 않은 아이디입니다.";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "비밀번호가 잘못되었습니다.";
                    break;
                case AuthError.UserNotFound:
                    failedMessage += "존재하지 않는 사용자입니다.";
                    break;
                default:
                    failedMessage = "짜잔 로그인 실패!";
                    break;
            }

            SetAlertMessage(failedMessage);
        }
        else
        {
            AuthResult authResult = loginTask.Result;
            FirebaseUser user = authResult.User;

            // 데이터베이스에서 사용자의 클리어 레벨 로드(이미 로그인후라 아이디나 이런거는 필요없음.. 근데 혹시몰라서 데베에 저장은 했습니당.)
            StartCoroutine(LoadUserData(user));
        }
    }

    private IEnumerator LoadUserData(FirebaseUser user)
    {
        var databaseTask = databaseReference.Child("Users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogError("사용자 데이터 로드 중 오류: " + databaseTask.Exception);
            SetAlertMessage("로그인 실패 - 사용자 정보를 가져오지 못했습니다.");
        }
        else
        {
            DataSnapshot snapshot = databaseTask.Result;

            if (snapshot != null && snapshot.Exists)
            {
                string id = snapshot.Child("id").Value.ToString();
                string nickname = snapshot.Child("nickname").Value.ToString();
                int clearLevel = int.Parse(snapshot.Child("clearLevel").Value.ToString());
;

                // User 클래스 초기화
                User loggedInUser = new User(id, nickname, clearLevel);

                PlayerManager.Instance.SetLoggedInUser(loggedInUser);

                SetAlertMessage($"로그인 성공 - 아이디: {loggedInUser.id}, 닉네임: {loggedInUser.nickname}");

                popupLogin.SetActive(false);
                titleButtons.SetActive(true);
                settingButton.SetActive(true);
            }
            else
            {
                Debug.LogError("사용자 데이터가 존재하지 않습니다.");
                SetAlertMessage("로그인 실패 - 사용자 정보를 찾을 수 없습니다.");
            }
        }
    }

    private void SetAlertMessage(string str)
    {
        alertMessage.text = str;
        alertMessage.gameObject.SetActive(true);
    }
}

// json을 이용한 로컬 불러오기
/*using System.IO;
using TMPro;
using UnityEngine;

public class LoginController_Json : MonoBehaviour
{
    public static LoginController_Json Instance;

    [Header("Field")]
    [SerializeField] private TMP_InputField idField;
    [SerializeField] private TMP_InputField pwField;

    [Header("Set Active Objects")]
    [SerializeField] private GameObject popupLogin;
    [SerializeField] private GameObject titleButtons;
    [SerializeField] private GameObject settingButton;
    [SerializeField] private TextMeshProUGUI alertMessage;

    private string userInfoFilePath;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        userInfoFilePath = Path.Combine(Application.persistentDataPath, "PlayerInfo.json");
        //SoundManager.Instance.PlayAudio(Sound.Bgm, "BGM");
    }

    public void OnLoginBtnClick()
    {
        string id = idField.text;
        string password = pwField.text;

        bool isIdValid = false;
        bool isPasswordValid = false;
        User2 loggedInUser = null;

        if (File.Exists(userInfoFilePath))
        {
            string json = File.ReadAllText(userInfoFilePath);
            UserList userList = JsonUtility.FromJson<UserList>(json);

            if (userList != null && userList.users != null)
            {
                foreach (var user in userList.users)
                {
                    if (user.id == id)
                    {
                        isIdValid = true;
                        if (user.password == password)
                        {
                            isPasswordValid = true;
                            loggedInUser = user;
                            break;
                        }
                    }
                }
            }
            SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
        }

        // 확인
        if (isIdValid && isPasswordValid)
        {
            PlayerManager_Json.Instance.SetLoggedInUser(loggedInUser);

            SetAlertMessage($"로그인 성공 - 아이디: {PlayerManager_Json.Instance.LoggedInUser.id}, 닉네임: {PlayerManager_Json.Instance.LoggedInUser.nickname}");

            popupLogin.SetActive(false);
            titleButtons.SetActive(true);
            settingButton.SetActive(true);
        }
        else
        {
            SetAlertMessage("로그인 실패 - [정보가 일치하지 않습니다.]");
        }
    }

    private void SetAlertMessage(string str)
    {
        alertMessage.text = str;
        alertMessage.gameObject.SetActive(true);
    }
}

*/