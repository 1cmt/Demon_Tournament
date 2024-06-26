using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIMultiMatchView : MonoBehaviourPunCallbacks
{
    NetworkManager networkManager;

    [Header("Player")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI player2Name;

    [Header("UI")]
    public GameObject matchButton;
    public TextMeshProUGUI matchButtonText;
    public GameObject cardView;
    public GameObject uiCondition;
    public SpriteRenderer background;
    public List<Sprite> backgrounds;

    private bool player1Ready = false;
    private bool player2Ready = false;

    int rand1;
    int rand2;

    private void Start()
    {
        networkManager = NetworkManager.Instance;
        SetMultiplayerCharacterInfo();
    }

    private void SetMultiplayerCharacterInfo()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // 플레이어1
            if (PhotonNetwork.IsMasterClient)
            {
                playerName.text = PlayerManager.Instance.LoggedInUser.nickname;
                photonView.RPC(nameof(SetPlayer1Info), RpcTarget.OthersBuffered, PlayerManager.Instance.LoggedInUser.nickname);
            }
            // 플레이어2
            else if (PhotonNetwork.PlayerList.Length > 1 && !PhotonNetwork.IsMasterClient)
            {
                player2Name.text = PlayerManager.Instance.LoggedInUser.nickname;
                photonView.RPC(nameof(SetPlayer2Info), RpcTarget.OthersBuffered, PlayerManager.Instance.LoggedInUser.nickname);
            }
        }
        else
        {
            Debug.Log("포톤 연결 이상");
        }
    }
    public void Disconnect()
    {
        Debug.Log("연결 끊기");
        networkManager.Disconnect();
        StartCoroutine(LoadIntroScene());
    }
    private IEnumerator LoadIntroScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
    public void OnMatchButtonClick()
    {
        matchButton.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            player1Ready = true;
        }
        else
        {
            player2Ready = true;
        }
        photonView.RPC(nameof(SyncPlayerReadyStatus), RpcTarget.All, player1Ready, player2Ready);

    }

    [PunRPC]
    private void SyncPlayerReadyStatus(bool p1Ready, bool p2Ready)
    {
        // 모든 클라이언트가 준비 상태를 수신받고 업데이트함
        player1Ready = p1Ready;
        player2Ready = p2Ready;

        // 두 플레이어 모두 준비 상태일 경우에만 캐릭터 생성
        if (player1Ready && player2Ready)
        {
            StartGame();
        }
    }

    [PunRPC]
    private void StartGame()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            rand1 = Random.Range(0, CharacterManager.Instance._characters.Count);
            rand2 = Random.Range(0, CharacterManager.Instance._characters.Count);
        }

        CharacterManager.Instance.playerCharacter = CharacterManager.Instance._characters[rand1];
        CharacterManager.Instance.player2Character = CharacterManager.Instance._characters[rand2];

        if (PhotonNetwork.IsMasterClient)
        {
            // 게임매니저에 필요한 UI 참조, 캐릭터 오브젝트 생성 및 초기화
            GameManager.Instance.StartStage();

            // 랜덤으로 배경 교체
            int rand = Random.Range(0, backgrounds.Count);
            background.sprite = backgrounds[rand];
        }

        // 카드뷰UI, 컨디션UI 활성화
        cardView.SetActive(true);
        uiCondition.SetActive(true);
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void SetPlayer1Info(string nickname)
    {
        playerName.text = nickname;
    }

    [PunRPC]
    private void SetPlayer2Info(string nickname)
    {
        player2Name.text = nickname;
    }
}
