using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance;
    
    [Header("포톤 설정")]
    public string gameVersion = "1.0";
    public int maxPlayersPerRoom = 2; // 테스트용으로 2명으로 변경
    
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
    
    // 플레이어 번호 할당 및 방에 저장
    private void AssignPlayerNumber()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 모든 플레이어에게 번호 할당
            AssignPlayerNumbersToAll();
        }
        else
        {
            // 마스터 클라이언트가 아닌 경우 할당된 번호 대기
            StartCoroutine(WaitForPlayerNumberAssignment());
        }
    }
    
    private void AssignPlayerNumbersToAll()
    {
        // 사용 가능한 번호들 (0부터 시작)
        List<int> availableNumbers = new List<int>();
        for (int i = 0; i < maxPlayersPerRoom; i++)
        {
            availableNumbers.Add(i);
        }
        
        // 각 플레이어에게 랜덤 번호 할당
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (availableNumbers.Count > 0)
            {
                int randomIndex = Random.Range(0, availableNumbers.Count);
                int playerNumber = availableNumbers[randomIndex];
                availableNumbers.RemoveAt(randomIndex);
                
                // 플레이어의 커스텀 프로퍼티에 번호 저장
                ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
                playerProps["PlayerNumber"] = playerNumber;
                player.SetCustomProperties(playerProps);
                
                Debug.Log($"플레이어 {player.NickName}에게 번호 {playerNumber} 할당됨");
            }
        }
        
        // 방의 커스텀 프로퍼티에 플레이어 번호 매핑 저장
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
        Dictionary<string, int> playerNumberMapping = new Dictionary<string, int>();
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("PlayerNumber"))
            {
                playerNumberMapping[player.UserId] = (int)player.CustomProperties["PlayerNumber"];
            }
        }
        
        roomProps["PlayerNumberMapping"] = playerNumberMapping;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        
        Debug.Log("플레이어 번호 할당 완료 및 방에 저장됨");
    }
    
    private System.Collections.IEnumerator WaitForPlayerNumberAssignment()
    {
        // 마스터 클라이언트가 번호를 할당할 때까지 대기
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerNumber"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        int assignedNumber = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"];
        Debug.Log($"플레이어 번호 할당됨: {assignedNumber}");
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
        
        // 플레이어 번호 할당 및 방에 저장
        AssignPlayerNumber();
        
        // 2명이 모이면 게임 시작
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