using System.Collections;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Photon.Pun;

[System.Serializable]
public class User
{
    public string id;
    public string nickname;
    public int clearLevel;
    public User()
    {
    }

    public User(string id, string nickname, int clearLevel)
    {
        this.id = id;
        this.nickname = nickname;
        this.clearLevel = clearLevel;
    }

    [PunRPC]
    public void InitializePlayerInfo(string id, string name, int level)
    {
        id = id;
        nickname = name;
        clearLevel = level;
    }
}

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static PlayerManager Instance { get; private set; }

    public User LoggedInUser { get; private set; }
    private PhotonView photonView;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetLoggedInUser(User user)
    {
        LoggedInUser = user;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC(nameof(InitializePlayerInfoRPC), RpcTarget.AllBuffered, LoggedInUser.id, LoggedInUser.nickname, LoggedInUser.clearLevel);
        }
        else
        {
            Debug.LogError("나는 서버에 재능이 없나보다");
        }
    }

    [PunRPC]
    private void InitializePlayerInfoRPC(string id, string name, int level)
    {
        LoggedInUser.InitializePlayerInfo(id, name, level);
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        LoggedInUser = null;
    }
}


// json으로 회원정보 저장하기
/*using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User2
{
    public string id;
    public string password;
    public string nickname;
    public int clearLevel;
}

[System.Serializable]
public class UserList
{
    public List<User2> users;
}

public class PlayerManager_Json : MonoBehaviour
{
    public static PlayerManager_Json Instance { get; private set; }
    public User2 LoggedInUser { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }   
    
    // 유저 정보 저장
    public void SetLoggedInUser(User2 user)
    {
        LoggedInUser = user;
        Debug.Log($"로그인 성공 - 아이디: {LoggedInUser.id}, 닉네임: {LoggedInUser.nickname}, Clear Level: {LoggedInUser.clearLevel}");
    }
}
*/