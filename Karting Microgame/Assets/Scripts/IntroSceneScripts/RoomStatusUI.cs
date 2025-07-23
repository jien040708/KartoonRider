using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class RoomStatusUI : MonoBehaviour
{
    public GameObject afterJoinPanel; // 방 참가 후 UI 패널
    public GameObject beforeJoinPanel; // 방 참가 전 UI 패널
    public Slider progressBar;
    public TMP_Text statusText;
    public TMP_Text roomCodeText;
    public Button leaveRoomButton;
    public Button deleteRoomButton;
    
    public int totalPlayers = 4; // 실제 방 최대 플레이어 수 로직
    public int currentPlayers = 0; // 실제 방 참가 플레이어 수 로직
    public string roomCode;

    public int hostUserId;        // 호스트 유저 ID
    public int currentUserId;     // 현재 로그인한 유저 ID
    //private const string apiBaseUrl = "https://kartoonrider-production-b878.up.railway.app"; // 너 API 주소로 바꿔줘

    private bool isGameStarting = false; // 게임 시작 중 상태

    string roomUUID = System.Guid.NewGuid().ToString(); // 예: "a1b2c3d4-e5f6-7g8h-9i0j"

    public void SetRoomCode(string code)
    {
        roomCode = code;
        roomCodeText.text = code; // UI에도 바로 반영
    }

    public void SetCurrentPlayers(int count)
    {
        currentPlayers = count;
        UpdatePlayerCount();
        
        // 4명이 모이면 시각적 피드백만 표시 (게임 시작은 PhotonRoomManager에서 처리)
        if (count >= 4 && !isGameStarting)
        {
            StartCoroutine(ShowGameReadyEffect());
        }
    }

    private IEnumerator ShowGameReadyEffect()
    {
        isGameStarting = true;
        
        // 상태 텍스트를 깜빡이는 효과
        for (int i = 0; i < 3; i++)
        {
            if (statusText != null)
            {
                statusText.text = "Ready!";
                statusText.color = Color.green;
            }
            yield return new WaitForSeconds(0.5f);
            
            if (statusText != null)
            {
                statusText.text = $"{currentPlayers} / {totalPlayers}";
                statusText.color = Color.white;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        if (statusText != null)
        {
            statusText.text = "GO!";
            statusText.color = Color.yellow;
        }
    }

    public void UpdatePlayerCount() // 웹소켓 또는 외부에서 호출
    {
        currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        float ratio = (float)currentPlayers / totalPlayers;
        progressBar.value = ratio;

        if (!isGameStarting)
        {
            statusText.text = $"{currentPlayers} / {totalPlayers}";
        }

        if (deleteRoomButton.gameObject.activeSelf)
        {
            ShowAfterCreatePanel();
        }

        if (leaveRoomButton.gameObject.activeSelf)
        {
            ShowAfterJoinPanel();
        }
    }

    public void ShowAfterJoinPanel() // 외부 패널 조절 스크립트에서 호출하기
    {
        float ratio = (float)currentPlayers / totalPlayers;
        progressBar.value = ratio;

        if (!isGameStarting)
        {
            statusText.text = $"{currentPlayers} / {totalPlayers}";
        }
        roomCodeText.text = roomCode;

        deleteRoomButton.gameObject.SetActive(false);
        leaveRoomButton.gameObject.SetActive(true);

        beforeJoinPanel.SetActive(false);
        afterJoinPanel.SetActive(true);
    }   

    public void ShowAfterCreatePanel() // 외부 패널 조절 스크립트에서 호출하기
    {
        float ratio = (float)currentPlayers / totalPlayers;
        progressBar.value = ratio;

        if (!isGameStarting)
        {
            statusText.text = $"{currentPlayers} / {totalPlayers}";
        }
        roomCodeText.text = roomCode;

        deleteRoomButton.gameObject.SetActive(false);
        leaveRoomButton.gameObject.SetActive(true);

        beforeJoinPanel.SetActive(false);
        afterJoinPanel.SetActive(true);
    }   

    public void OnLeaveRoomButtonClicked()
    {
        Debug.Log("Leave Room Button Clicked");
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            beforeJoinPanel.SetActive(true);
            afterJoinPanel.SetActive(false);
            Debug.Log("🚪 방 나가기 요청 전송");
        }
        else
        {
            Debug.LogWarning("❗ 현재 방에 있지 않음");
        }
    }

    public void OnDeleteRoomButtonClicked()
    {
        Debug.Log("Delete Room Button Clicked");
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            beforeJoinPanel.SetActive(true);
            afterJoinPanel.SetActive(false);
            Debug.Log("🚪 방 나가기 요청 전송");
        }
        else
        {
            Debug.LogWarning("❗ 현재 방에 있지 않음");
        }
    }
    // IEnumerator CallLeaveRoomAPI()
    // {
    //     string url = $"{apiBaseUrl}/rooms/leave/{roomCode}";

    //     // ✅ 현재 로그인된 유저 ID (UserManager에서 받아옴)
    //     int userId = UserManager.Instance.UserId;
    //     string json = $"{{\"user_id\": {userId}}}";

    //     UnityWebRequest request = new UnityWebRequest(url, "POST");
    //     byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
    //     request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //     request.downloadHandler = new DownloadHandlerBuffer();
    //     request.SetRequestHeader("Content-Type", "application/json");

    //     yield return request.SendWebRequest();

    //     if (request.result == UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("방 나가기 성공");
    //         beforeJoinPanel.SetActive(true);
    //         afterJoinPanel.SetActive(false);
    //     }
    //     else
    //     {
    //         Debug.LogError("방 나가기 실패: " + request.error + " / " + request.downloadHandler.text);
    //     }
    // }

    // IEnumerator CallDeleteRoomAPI()
    // {
    //     string url = $"{apiBaseUrl}/rooms/delete/{roomCode}"; // 방 삭제 API URL
    //     UnityWebRequest request = UnityWebRequest.Delete(url);
    //     yield return request.SendWebRequest();

    //     if (request.result == UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("방 삭제 성공");
    //         beforeJoinPanel.SetActive(true);
    //         afterJoinPanel.SetActive(false);
        
    //     else
    //     {
    //         Debug.LogError("방 삭제 실패: " + request.error);
    //     }
    // }

    void Start()
    {
        afterJoinPanel.SetActive(false);
        deleteRoomButton.gameObject.SetActive(false);
        leaveRoomButton.gameObject.SetActive(false);
    }

    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.5f; // 0.5초마다 업데이트
    
    // Update is called once per frame
    void Update()
    {
        // 방에 있을 때만 플레이어 수 업데이트 (성능 최적화)
        if (PhotonNetwork.InRoom && Time.time - lastUpdateTime > UPDATE_INTERVAL)
        {
            UpdatePlayerCount();
            lastUpdateTime = Time.time;
        }
    }
}
