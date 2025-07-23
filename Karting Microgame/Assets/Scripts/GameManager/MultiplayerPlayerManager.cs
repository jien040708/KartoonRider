using UnityEngine;
using System.Collections.Generic;
using KartGame.KartSystems;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MultiplayerPlayerManager : MonoBehaviourPun
{
    [Header("플레이어 설정")]
    public GameObject player1; // Player_1
    public GameObject player2; // Player_2
    public GameObject player3; // Player_3
    public GameObject player4; // Player_4
    
    [Header("플레이어 정보")]
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private GameObject currentPlayer;
    private int currentPlayerId = -1;
    
    // 플레이어 번호 할당을 위한 룸 프로퍼티 키
    private const string PLAYER_NUMBERS_KEY = "PlayerNumbers";
    
    void Awake()
    {
        // PhotonView가 없으면 자동으로 추가
        if (GetComponent<PhotonView>() == null)
        {
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 998; // 고유한 ViewID 설정
            Debug.Log("MultiplayerPlayerManager에 PhotonView 추가됨");
        }
        
        // 포톤 콜백 구독
        if (photonView != null)
        {
            photonView.ObservedComponents.Add(this);
        }
        
        // PlayerNumbering 이벤트 구독
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }
    
    void OnDestroy()
    {
        // PlayerNumbering 이벤트 구독 해제
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }
    
    // 플레이어 번호가 변경될 때 호출되는 콜백
    public void OnPlayerNumberingChanged()
    {
        Debug.Log("플레이어 번호가 변경되었습니다!");
        // 현재 플레이어 설정 다시 적용
        SetupCurrentPlayer();
        DisableOtherPlayers();
    }
    
    void Start()
    {
        // Inspector에서 연결되지 않은 경우 자동으로 찾기
        if (player1 == null) player1 = GameObject.Find("Player_1");
        if (player2 == null) player2 = GameObject.Find("Player_2");
        if (player3 == null) player3 = GameObject.Find("Player_3");
        if (player4 == null) player4 = GameObject.Find("Player_4");
        
        // 플레이어들 등록
        RegisterPlayers();
        
        // 현재 플레이어 설정
        SetupCurrentPlayer();
        
        // 다른 플레이어들 조작 불가능하게 설정
        DisableOtherPlayers();
    }
    
    void RegisterPlayers()
    {
        // null 체크 추가
        if (player1 == null) Debug.LogError("Player 1이 연결되지 않았습니다!");
        if (player2 == null) Debug.LogError("Player 2가 연결되지 않았습니다!");
        if (player3 == null) Debug.LogError("Player 3이 연결되지 않았습니다!");
        if (player4 == null) Debug.LogError("Player 4가 연결되지 않았습니다!");
        
        players[1] = player1;
        players[2] = player2;
        players[3] = player3;
        players[4] = player4;
        
        Debug.Log("플레이어들 등록 완료");
    }
    
    void SetupCurrentPlayer()
    {
        // 포톤 플레이어 번호 시스템 사용
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        Debug.Log($"포톤 플레이어 번호: {playerNumber}");
        
        if (players.ContainsKey(playerNumber + 1)) // 0-based를 1-based로 변환
        {
            currentPlayer = players[playerNumber + 1];
            
            // 카메라가 현재 플레이어를 따라가도록 설정
            var camera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            if (camera != null && currentPlayer != null)
            {
                camera.Follow = currentPlayer.transform;
                camera.LookAt = currentPlayer.transform;
                Debug.Log($"카메라가 {currentPlayer.name} (플레이어 {playerNumber + 1})을 따라갑니다");
            }
        }
        else
        {
            Debug.LogWarning($"플레이어 번호 {playerNumber + 1}에 해당하는 플레이어가 없습니다!");
        }
    }
    
    void DisableOtherPlayers()
    {
        int currentPlayerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        
        foreach (var kvp in players)
        {
            if (kvp.Key != (currentPlayerNumber + 1) && kvp.Value != null) // 0-based를 1-based로 변환
            {
                var kart = kvp.Value.GetComponent<ArcadeKart>();
                if (kart != null)
                {
                    kart.SetCanMove(false); // 조작만 비활성화
                    // ArcadeKart 컴포넌트는 활성화 유지 (포톤 동기화를 위해)
                    Debug.Log($"{kvp.Value.name} 조작 비활성화 (동기화는 유지)");
                }
                else
                {
                    Debug.LogError($"{kvp.Value.name}에 ArcadeKart 컴포넌트가 없습니다!");
                }
                
                // KartPlayerAnimator는 비활성화 (에러 방지)
                var animator = kvp.Value.GetComponent<KartGame.KartSystems.KartPlayerAnimator>();
                if (animator != null)
                {
                    animator.enabled = false;
                    Debug.Log($"{kvp.Value.name} 애니메이터 비활성화");
                }
            }
            else if (kvp.Value == null)
            {
                Debug.LogError($"플레이어 {kvp.Key}가 null입니다!");
            }
        }
    }
    
    // 다른 플레이어의 위치 업데이트 (포톤 자동 동기화로 처리됨)
    public void UpdatePlayerPosition(int playerId, Vector3 position, Quaternion rotation)
    {
        // PhotonTransformView가 자동으로 처리하므로 별도 로직 불필요
        Debug.Log($"플레이어 {playerId} 위치 업데이트 (포톤 자동 동기화)");
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
    
    // 현재 플레이어의 위치를 포톤으로 전송 (PhotonTransformView가 자동으로 처리)
    public void SendCurrentPlayerPosition()
    {
        // PhotonTransformView가 자동으로 위치 동기화를 처리하므로 
        // 별도의 전송 로직이 필요하지 않습니다.
        Debug.Log("포톤 자동 위치 동기화 사용 중");
    }
} 