using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Analytics;
using System;

public class RoomManager : MonoBehaviour
{
    public RoomStatusUI roomStatusUI;
    public GameObject afterJoinPanel; // 방 참가 후 UI 패널
    public GameObject beforeJoinPanel; // 방 참가 전 UI 패널

    public GameObject Player;
    public GameObject popupCreateResult;
    public TMP_Text roomCodeText;

    public GameObject popupJoinInput;
    public Button joinConfirmButton;
    public Button joinCancelButton;
    public Button createConfirmButton;
    public GameObject popupJoinFailedWarn;
    public Button createFailedButton;
    public TMP_InputField joinCodeInput;

    public GameObject popupBackgroundOverlay;

    string inputCode;

    public void OnClickCreateRoom()
    {
        StartCoroutine(CreateRoomRequest());
         // 플레이어 오브젝트 비활성화
    }

    public void OnClickCreateConfirm()
    {
        popupCreateResult.SetActive(false);
        popupBackgroundOverlay.SetActive(false);
        Player.SetActive(true); // 플레이어 오브젝트 활성화
        roomStatusUI.ShowAfterCreatePanel();
    }

    public void OnClickCreateFailed()
    {
        createFailedButton.gameObject.SetActive(false);
        Player.SetActive(true); // 플레이어 오브젝트 활성화
    }

    public void OnClickJoinRoom()
    {   
        popupJoinInput.SetActive(true);
        popupBackgroundOverlay.SetActive(true);
        Player.SetActive(false); // 플레이어 오브젝트 비활성화
    }

    public void OnJoinCodeChanged(string value)
    {
        inputCode = value;
        Debug.Log("현재 입력값: " + inputCode);
    }

    public void OnClickJoinConfirm()
    {
        StartCoroutine(JoinRoomRequest(inputCode));
    }

    public void OnClickJoinCancel()
    {
        popupJoinInput.SetActive(false);
        popupBackgroundOverlay.SetActive(false);
        if (popupJoinFailedWarn.activeSelf)
        {
            popupJoinFailedWarn.SetActive(false);
        }
        Player.SetActive(true); // 플레이어 오브젝트 활성화
    }

    IEnumerator CreateRoomRequest()
    {
        int host_id = UserManager.Instance.UserId;
        string url = $"https://kartoonrider-production-b878.up.railway.app/rooms/create/{host_id}";

        // body: {"name": "MyRoom"}
        string json = "{\"name\":\"MyRoom\"}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string result = req.downloadHandler.text;
            RoomCodeResponse data = JsonUtility.FromJson<RoomCodeResponse>(result);
            roomCodeText.text = data.room_code;
            roomStatusUI.SetRoomCode(data.room_code);


            RoomWebSocket.Instance.Connect(data.room_code, host_id.ToString());

            Player.SetActive(false);
            popupCreateResult.SetActive(true);
            popupBackgroundOverlay.SetActive(true);
        }
        else
        {
            Debug.LogError("방 생성 실패: " + req.responseCode + " / " + req.error);
            Player.SetActive(false);
            createFailedButton.gameObject.SetActive(true);
        }
    }

    [System.Serializable]
    public class RoomCodeResponse
    {
        public string room_code;
    }



    IEnumerator JoinRoomRequest(string code)
    {
        int userId = UserManager.Instance.UserId;
        string url = $"https://kartoonrider-production-b878.up.railway.app/rooms/join/{code}";

        // body: {"user_id": 1}
        string json = $"{{\"user_id\":{userId}}}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("참가 성공");
            roomStatusUI.SetRoomCode(code);

            popupJoinInput.SetActive(false);
            popupBackgroundOverlay.SetActive(false);
            Player.SetActive(true);

            roomStatusUI.SetRoomCode(code); //복귀2
            roomStatusUI.ShowAfterJoinPanel();

            RoomWebSocket.Instance.Connect(code, userId.ToString());
        }
        else
        {
            Debug.LogError("참가 실패: " + req.responseCode + " / " + req.error);
            popupJoinFailedWarn.SetActive(true);
        }
    }




    void Start(){
        popupJoinInput.SetActive(false);
        popupBackgroundOverlay.SetActive(false);  
        popupJoinFailedWarn.SetActive(false);
        popupCreateResult.SetActive(false);
        createFailedButton.gameObject.SetActive(false);

        joinCodeInput.onValueChanged.AddListener(OnJoinCodeChanged);

    }
}
