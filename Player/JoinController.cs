using System.Collections;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class JoinController : MonoBehaviour
{
    public static JoinController Instance;

    [Header("Field")]
    [SerializeField] private TMP_InputField idField;
    [SerializeField] private TMP_InputField pwField;
    [SerializeField] private TMP_InputField nameField;

    [Header("Set Active Objects")]
    [SerializeField] private GameObject popupSignup;
    [SerializeField] private GameObject popupLogin;
    [SerializeField] private TextMeshProUGUI alertMessage;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    private void Awake()
    {
        Instance = this;
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.GetInstance("https://demontournament-32fb6-default-rtdb.firebaseio.com/").RootReference;
    }

    public void OnJoinBtnClick()
    {
        string id = idField.text + "@demon.com";
        string password = pwField.text;
        string nickname = nameField.text;

        bool isPasswordValid = PasswordCheck(password);
        bool isNameValid = NameCheck(nickname);

        if (isPasswordValid && isNameValid)
        {
            StartCoroutine(JoinAsync(id, password, nickname));
        }
    }

    bool PasswordCheck(string password)
    {
        if (password.Length >= 4 && password.Length <= 12)
        {
            Debug.Log($"입력한 비밀번호: {password}");
            return true;
        }
        else
        {
            SetAlertMessage("회원가입 실패 - [비밀번호는 4~12 글자 이내로 입력해야합니다.]");
            return false;
        }
    }

    bool NameCheck(string name)
    {
        if (name.Length >= 1 && name.Length <= 10)
        {
            return true;
        }
        else
        {
            SetAlertMessage("회원가입 실패 - [닉네임은 1~10 글자 이내로 입력해야합니다.]");
            return false;
        }
    }

    private IEnumerator JoinAsync(string id, string password, string displayName)
    {
        var joinTask = auth.CreateUserWithEmailAndPasswordAsync(id, password);

        yield return new WaitUntil(() => joinTask.IsCompleted);

        if (joinTask.Exception != null)
        {
            Debug.LogError(joinTask.Exception);
            SetAlertMessage("회원가입 실패");
        }
        else
        {
            AuthResult authResult = joinTask.Result;
            FirebaseUser user = authResult.User;

            // 사용자 데이터 데이터베이스에 저장
            id = id.Substring(0, id.IndexOf('@')); // 아이디 저장할때 데베에는 이메일 제외하고 저장하기
            User userData = new User(id, displayName, 0); // 회원가입시 클리어 레벨 0으로 초기화
            string json = JsonUtility.ToJson(userData);
            var task = databaseReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(json);

            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError("사용자 데이터 저장 실패: " + task.Exception.Message);
                SetAlertMessage("회원가입 실패 - 데이터 저장 오류");
            }
            else
            {
                SetAlertMessage($"{displayName} 님 회원가입 성공");
            }
        }
    }

    private void SetAlertMessage(string str)
    {
        alertMessage.text = str;
        alertMessage.gameObject.SetActive(true);
    }
}

// json을 이용한 로컬 저장
/*using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class JoinController_Json : MonoBehaviour
{
    public static JoinController_Json Instance;

    [Header("Field")]
    [SerializeField] private TMP_InputField idField;
    [SerializeField] private TMP_InputField pwField;
    [SerializeField] private TMP_InputField nameField;

    [Header("Set Active Objects")]
    [SerializeField] private GameObject popupSignup;
    [SerializeField] private GameObject popupLogin;
    [SerializeField] private TextMeshProUGUI alertMessage;

    private string userInfoFilePath;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        userInfoFilePath = Path.Combine(Application.persistentDataPath, "PlayerInfo.json");

        if (!File.Exists(userInfoFilePath))
        {
            UserList emptyUserList = new UserList { users = new List<User2>() };
            string emptyJson = JsonUtility.ToJson(emptyUserList, true);
            File.WriteAllText(userInfoFilePath, emptyJson);
        }

    }

    public void OnJoinBtnClick()
    {
        string id = idField.text;
        string password = pwField.text;
        string nickname = nameField.text;

        bool isIdValid = !IdCheck(id);
        bool isPasswordValid = PasswordCheck(password);
        bool isNameValid = NameCheck(nickname);

        if (isIdValid && isPasswordValid && isNameValid)
        {
            SaveUser(id, password, nickname);

            SetAlertMessage("회원정보 저장");

            popupSignup.SetActive(false);

            Debug.Log("회원정보 저장");
        }

        SoundManager.Instance.PlayAudio(Sound.Effect, "ButtonClick");
    }

    public void OnClickSigninButton()
    {
        popupSignup.SetActive(false);
        popupLogin.SetActive(true);
    }

    // 유효성 검사
    bool IdCheck(string id)
    {
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
                        SetAlertMessage("회원가입 실패 - [이미 존재하는 아이디입니다.]");

                        Debug.Log("회원가입 실패 - [이미 존재하는 아이디입니다.]");
                        return true;
                    }
                }
            }
        }

        if (id.Length < 1)
        {
            SetAlertMessage("회원가입 실패 - [아이디는 한글자 이상 입력해야합니다.]");

            Debug.Log("회원가입 실패 - [아이디는 한글자 이상 입력해야합니다.]");
            return true;
        }
        return false;
    }

    bool PasswordCheck(string password)
    {
        if (password.Length >= 4 && password.Length <= 12)
        {
            Debug.Log($"입력한 비밀번호: {password}");
            return true;
        }
        else
        {
            SetAlertMessage("회원가입 실패 - [비밀번호는 4~12 글자 이내로 입력해야합니다.]");

            Debug.Log("회원가입 실패 - [비밀번호는 4~12 글자 이내로 입력해야합니다.]");
            return false;
        }
        
    }

    bool NameCheck(string name)
    {
        if (name.Length >= 1 && name.Length <= 10)
        {
            return true;
        }
        else
        {
            SetAlertMessage("회원가입 실패 - [닉네임는 1~10 글자 이내로 입력해야합니다.]");

            Debug.Log("회원가입 실패 - [닉네임는 1~10 글자 이내로 입력해야합니다.]");
            return false;
        }
    }

    // Json에 User 저장하기
    void SaveUser(string id, string password, string name)
    {
        if (string.IsNullOrEmpty(userInfoFilePath))
        {
            return;
        }

        User2 newUser = new User2
        {
            id = id,
            password = password,
            nickname = name,
            clearLevel = 0
        };
        UserList userList;

        if (File.Exists(userInfoFilePath))
        {
            string json = File.ReadAllText(userInfoFilePath);

            if (string.IsNullOrEmpty(json))
            {
                userList = new UserList { users = new List<User2>() };
            }
            else
            {
                userList = JsonUtility.FromJson<UserList>(json);
            }
        }
        else
        {
            userList = new UserList { users = new List<User2>() };
        }

        if (userList.users == null)
        {
            userList.users = new List<User2>();
        }

        userList.users.Add(newUser);
        string newJson = JsonUtility.ToJson(userList, true);
        File.WriteAllText(userInfoFilePath, newJson);
    }

    private void SetAlertMessage(string str)
    {
        alertMessage.text = str;
        alertMessage.gameObject.SetActive(true);
    }
}

*/