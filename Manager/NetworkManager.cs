using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager _instance;
    public static NetworkManager Instance { get { return _instance; } }

    [SerializeField] private int mainGameSceneIndex = 2; // 씬 인덱스
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomListParent;
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomListEntries = new Dictionary<string, GameObject>();

    private bool isConnecting = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        ConnectToPhoton();
    }

    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            isConnecting = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버에 연결됨");
        PhotonNetwork.JoinLobby();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
            TypedLobby lobby = new TypedLobby("", LobbyType.Default);
            PhotonNetwork.GetCustomRoomList(lobby, "");
        }
        else
        {
            Debug.LogError("마스터 서버에 연결되지 않음");
        }
    }

    private void ClearRoomList()
    {
        // Clear existing room entries in UI
        foreach (Transform child in roomListParent)
        {
            Destroy(child.gameObject);
        }

        cachedRoomList.Clear();
        roomListEntries.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"현재 방 개수: {roomList.Count}개");

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }

            // Update or add room info to cache
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomPrefab, roomListParent);
            entry.transform.localScale = Vector3.one;

            RoomData roomData = entry.GetComponent<RoomData>();
            if (roomData != null)
            {
                roomData.Initialize(info);
            }
            else
            {
                Debug.LogError("룸 데이터 초기화 오류");
            }

            roomListEntries.Add(info.Name, entry);
        }
    }

    public void CreateRoom()
    {
        int maxPlayers = 2;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers };
            PhotonNetwork.CreateRoom(PlayerManager.Instance.LoggedInUser.nickname.ToString(), roomOptions, TypedLobby.Default);
            Debug.Log("방 생성 완료");
        }
        else
        {
            ConnectToPhoton();
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("방이 생성됨. 리스트 새로 고침...");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("방에 입장함. 씬 로드...");
        PhotonNetwork.LoadLevel(mainGameSceneIndex);
        //UIMatchView.Instance.SetMultiplayerCharacterInfo();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("로비로 돌아가서 새로 고침...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비에 입장함. 리스트 새로 고침...");
        RefreshRoomList();
    }  
    
    // 연결 끊기
    public void Disconnect() => PhotonNetwork.Disconnect();
    // Disconnect 이벤트 콜백
    public override void OnDisconnected(DisconnectCause cause) => Debug.Log("끊김");
}
