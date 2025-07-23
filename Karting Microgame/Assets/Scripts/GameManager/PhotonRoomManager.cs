using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance;
    
    [Header("포톤 설정")]
    public string gameVersion = "1.0";
    public int maxPlayersPerRoom = 4;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // PhotonView가 없으면 자동으로 추가
            if (GetComponent<PhotonView>() == null)
            {
                PhotonView photonView = gameObject.AddComponent<PhotonView>();
                photonView.ViewID = 999; // 고유한 ViewID 설정
                Debug.Log("PhotonRoomManager에 PhotonView 추가됨");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 포톤 연결
        PhotonNetwork.AutomaticallySyncScene = true;
        ConnectToPhoton();
    }
    
    void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("포톤 서버에 연결 중...");
        }
    }
    
    public void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"방 생성 중: {roomName}");
    }
    
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        Debug.Log($"방 참가 중: {roomName}");
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("방을 나갑니다.");
    }
    
    // 포톤 콜백들
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 마스터 서버에 연결됨");
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log($"방에 참가됨: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        
        // 4명이 모이면 게임 시작
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayersPerRoom)
        {
            StartGame();
        }
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"새 플레이어 입장: {newPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        
        // 4명이 모이면 게임 시작
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayersPerRoom)
        {
            StartGame();
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
    }
    
    public override void OnLeftRoom()
    {
        Debug.Log("방을 나갔습니다.");
        // 로비로 돌아가기
        SceneManager.LoadScene("IntroScene");
    }
    
    private void StartGame()
    {
        Debug.Log("게임을 시작합니다!");
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트만 씬 전환
            PhotonNetwork.LoadLevel("MainScene");
        }
    }
    
    public void SendPlayerFinished()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC("OnPlayerFinished", RpcTarget.All);
        }
    }
    
    public void SendGameEnd()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC("OnGameEnd", RpcTarget.All);
        }
    }
    
    [PunRPC]
    void OnPlayerFinished()
    {
        Debug.Log("🏆 플레이어가 결승선을 통과했습니다!");
        // 여기에 결승선 통과 UI 표시 등의 로직 추가
    }
    
    [PunRPC]
    void OnGameEnd()
    {
        Debug.Log("🏁 게임이 종료되었습니다!");
        // 모든 플레이어를 로비로 돌려보내기
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("IntroScene");
        }
    }
} 