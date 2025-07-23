using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PlayerNumberingManager : MonoBehaviour
{
    [Header("PlayerNumbering 설정")]
    public bool dontDestroyOnLoad = true;
    
    private PlayerNumbering playerNumbering;
    
    void Awake()
    {
        // PlayerNumbering 컴포넌트가 없으면 자동으로 추가
        playerNumbering = FindObjectOfType<PlayerNumbering>();
        if (playerNumbering == null)
        {
            GameObject playerNumberingObj = new GameObject("PlayerNumbering");
            playerNumbering = playerNumberingObj.AddComponent<PlayerNumbering>();
            playerNumbering.dontDestroyOnLoad = dontDestroyOnLoad;
            Debug.Log("PlayerNumbering 컴포넌트가 자동으로 생성되었습니다.");
        }
        
        // PlayerNumbering 이벤트 구독
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }
    
    void OnDestroy()
    {
        // PlayerNumbering 이벤트 구독 해제
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }
    
    void OnPlayerNumberingChanged()
    {
        Debug.Log("PlayerNumbering 변경됨!");
        
        // 모든 플레이어의 번호 출력
        if (PlayerNumbering.SortedPlayers != null)
        {
            foreach (Player player in PlayerNumbering.SortedPlayers)
            {
                Debug.Log($"플레이어 {player.NickName} (ActorNumber: {player.ActorNumber}) -> 번호: {player.GetPlayerNumber()}");
            }
        }
        
        // MultiplayerPlayerManager에 알림
        var playerManager = FindObjectOfType<MultiplayerPlayerManager>();
        if (playerManager != null)
        {
            playerManager.OnPlayerNumberingChanged();
        }
    }
    
    void Start()
    {
        // 포톤 연결 확인
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("포톤에 연결되지 않았습니다. PlayerNumbering이 작동하지 않을 수 있습니다.");
        }
        else
        {
            Debug.Log("PlayerNumbering 시스템이 초기화되었습니다.");
        }
    }
    
    void Update()
    {
        // 방에 있을 때만 PlayerNumbering 상태 확인
        if (PhotonNetwork.InRoom && playerNumbering != null)
        {
            // PlayerNumbering이 제대로 작동하는지 확인
            if (PhotonNetwork.LocalPlayer.GetPlayerNumber() < 0)
            {
                // 번호가 할당되지 않았으면 새로고침
                playerNumbering.RefreshData();
            }
        }
    }
} 