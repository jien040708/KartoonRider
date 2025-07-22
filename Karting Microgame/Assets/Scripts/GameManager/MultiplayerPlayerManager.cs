using UnityEngine;
using System.Collections.Generic;
using KartGame.KartSystems;

public class MultiplayerPlayerManager : MonoBehaviour
{
    [Header("플레이어 설정")]
    public GameObject player1; // Player_1
    public GameObject player2; // Player_2
    public GameObject player3; // Player_3
    public GameObject player4; // Player_4
    
    [Header("플레이어 정보")]
    public int currentPlayerId = 1; // 현재 플레이어 ID (기본값)
    
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private GameObject currentPlayer;
    
    void Start()
    {
        // 플레이어들 등록
        RegisterPlayers();
        
        // 현재 플레이어 설정
        SetupCurrentPlayer();
        
        // 다른 플레이어들 조작 불가능하게 설정
        DisableOtherPlayers();
    }
    
    void RegisterPlayers()
    {
        players[1] = player1;
        players[2] = player2;
        players[3] = player3;
        players[4] = player4;
        
        Debug.Log("플레이어들 등록 완료");
    }
    
    void SetupCurrentPlayer()
    {
        // 서버에서 할당된 플레이어 ID에 따라 현재 플레이어 설정
        if (players.ContainsKey(currentPlayerId))
        {
            currentPlayer = players[currentPlayerId];
            
            // 카메라가 현재 플레이어를 따라가도록 설정
            var camera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            if (camera != null && currentPlayer != null)
            {
                camera.Follow = currentPlayer.transform;
                camera.LookAt = currentPlayer.transform;
                Debug.Log($"카메라가 {currentPlayer.name} (플레이어 {currentPlayerId})을 따라갑니다");
            }
        }
        else
        {
            Debug.LogWarning($"플레이어 ID {currentPlayerId}에 해당하는 플레이어가 없습니다!");
        }
    }
    
    void DisableOtherPlayers()
    {
        foreach (var kvp in players)
        {
            if (kvp.Key != currentPlayerId)
            {
                var kart = kvp.Value.GetComponent<ArcadeKart>();
                if (kart != null)
                {
                    kart.SetCanMove(false);
                    // ArcadeKart 컴포넌트 자체를 비활성화
                    kart.enabled = false;
                    Debug.Log($"{kvp.Value.name} 조작 비활성화");
                }
            }
        }
    }
    
    // 다른 플레이어의 위치 업데이트 (웹소켓에서 받은 정보로)
    public void UpdatePlayerPosition(int playerId, Vector3 position, Quaternion rotation)
    {
        if (players.ContainsKey(playerId) && playerId != currentPlayerId)
        {
            players[playerId].transform.position = position;
            players[playerId].transform.rotation = rotation;
        }
    }
    
    // 플레이어 ID 설정
    public void SetCurrentPlayerId(int playerId)
    {
        currentPlayerId = playerId;
        Debug.Log($"플레이어 ID 설정됨: {playerId}");
        
        // 플레이어 설정 다시 적용
        SetupCurrentPlayer();
        DisableOtherPlayers();
    }
    
    // 현재 플레이어의 위치를 웹소켓으로 전송
    public void SendCurrentPlayerPosition()
    {
        if (currentPlayer != null)
        {
            var roomWebSocketObj = GameObject.Find("RoomWebSocketManager");
            if (roomWebSocketObj != null)
            {
                Vector3 position = currentPlayer.transform.position;
                Quaternion rotation = currentPlayer.transform.rotation;
                string message = $"__PLAYER_POSITION__:{currentPlayerId}:{position.x:F2},{position.y:F2},{position.z:F2}:{rotation.x:F2},{rotation.y:F2},{rotation.z:F2},{rotation.w:F2}";
                roomWebSocketObj.SendMessage("SendMessage", message);
            }
        }
    }
} 