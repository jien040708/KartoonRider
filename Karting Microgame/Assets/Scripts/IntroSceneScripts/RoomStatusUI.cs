using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class RoomStatusUI : MonoBehaviour
{
    public GameObject afterJoinPanel; // 방 참가 후 UI 패널
    public GameObject beforeJoinPanel; // 방 참가 전 UI 패널
    public Slider progressBar;
    public TMP_Text statusText;
    public TMP_Text roomNameText;
    public Button leaveRoomButton;
    public Button deleteRoomButton;
    
    public int totalPlayers = 4; // 실제 방 최대 플레이어 수 로직
    private int currentPlayers = 0; // 실제 방 참가 플레이어 수 로직
    private string roomName = "Room Name"; // 실제 방 이름 로직

    public int roomId;            // 방 ID
    public int hostUserId;        // 호스트 유저 ID
    public int currentUserId;     // 현재 로그인한 유저 ID
    private const string apiBaseUrl = "https://kartoonrider-production.up.railway.app/"; // 너 API 주소로 바꿔줘


    public void UpdatePlayerCount() //웹소켓으로 호출되는 메소드
    {
        // 이 메소드는 웹소켓으로부터 호출되어 현재 플레이어 수를 업데이트합니다.
        // 예시로 임의의 값으로 업데이트합니다.
        currentPlayers = Random.Range(1, totalPlayers + 1); // 예시로 랜덤하게 플레이어 수를 설정
        ShowAfterJoinPanel(); // UI 업데이트
    }


    public void ShowAfterJoinPanel() // 외부 패널 조절 스크립트에서 호출하기
    {
        float ratio = (float)currentPlayers / totalPlayers;
        progressBar.value = ratio;

        statusText.text = $"{currentPlayers} / {totalPlayers}";
        roomNameText.text = roomName;

        deleteRoomButton.gameObject.SetActive(false);
        leaveRoomButton.gameObject.SetActive(true);

        beforeJoinPanel.SetActive(false);
        afterJoinPanel.SetActive(true);
    }   

    public void ShowAfterCreatePanel() // 외부 패널 조절 스크립트에서 호출하기
    {
        float ratio = (float)currentPlayers / totalPlayers;
        progressBar.value = ratio;

        statusText.text = $"{currentPlayers} / {totalPlayers}";
        roomNameText.text = roomName;

        deleteRoomButton.gameObject.SetActive(true);
        leaveRoomButton.gameObject.SetActive(false);

        beforeJoinPanel.SetActive(false);
        afterJoinPanel.SetActive(true);
    }   


    public void OnLeaveRoomButtonClicked()
    {
        Debug.Log("Leave Room Button Clicked");
        StartCoroutine(CallLeaveRoomAPI());
    }

    public void OnDeleteRoomButtonClicked()
    {
        Debug.Log("Delete Room Button Clicked");
        StartCoroutine(CallDeleteRoomAPI());
    }

    IEnumerator CallLeaveRoomAPI()
    {
        string url = $"{apiBaseUrl}/rooms/leave/{roomId}";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("방 나가기 성공");
            beforeJoinPanel.SetActive(true);
            afterJoinPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("방 나가기 실패: " + request.error);
        }
    }

    IEnumerator CallDeleteRoomAPI()
    {
        string url = $"{apiBaseUrl}/{roomId}"; // 방 삭제 API URL
        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("방 삭제 성공");
            beforeJoinPanel.SetActive(true);
            afterJoinPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("방 삭제 실패: " + request.error);
        }
    }

    void Start()
    {
        afterJoinPanel.SetActive(false);
        deleteRoomButton.gameObject.SetActive(false);
        leaveRoomButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
