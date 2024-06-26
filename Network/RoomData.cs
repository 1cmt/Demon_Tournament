using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomData : MonoBehaviour
{
    public GameObject prefab;

    [SerializeField] private TextMeshProUGUI roomNameText; 
    [SerializeField] private Button joinButton;

    private string roomName; // 방 이름

    public void Initialize(RoomInfo roomInfo)
    {
        roomName = roomInfo.Name;

        GameObject instantiatedPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        DispRoomData();
    }

    // 임시
    public void DispRoomData()
    {
        roomNameText.text = roomName;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(JoinRoom);
    }

    // 임시

    void JoinRoom()
    {
        PhotonNetwork.NickName = "다죽인다" + Random.Range(0, 999); // 임시
        PlayerPrefs.SetString("USER_ID", PhotonNetwork.NickName); 
        PhotonNetwork.JoinRoom(roomName); // 선택한 방으로 입장
    }

    public static RoomData CreateInstance(GameObject prefab, Transform parent, RoomInfo roomInfo)
    {
        GameObject instance = Instantiate(prefab, parent);
        RoomData roomData = instance.GetComponent<RoomData>();
        if (roomData != null)
        {
            roomData.Initialize(roomInfo);
        }
        else
        {
            Debug.LogError("Prefab에 RoomData 컴포넌트가 없습니다.");
        }
        return roomData;
    }
}
