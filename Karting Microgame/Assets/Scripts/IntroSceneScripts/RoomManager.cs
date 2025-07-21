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

    public void OnJoinCodeChanged(String inputCode)
    {
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
        roomStatusUI.ShowAfterCreatePanel();
    }

    IEnumerator CreateRoomRequest()
    {
        int host_id = 1; //실제 로그인 한 유저 id 갖고 오는 로직 추가
        UnityWebRequest req = UnityWebRequest.Get($"https://kartoonrider-production.up.railway.app/rooms/create/host_id={host_id}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string roomCode = req.downloadHandler.text;
            roomCodeText.text = roomCode;
            Player.SetActive(false);
            popupCreateResult.SetActive(true);
            popupBackgroundOverlay.SetActive(true);
        }
        else
        {
            Debug.LogError("방 생성 실패: " + req.error);
            Player.SetActive(false);
            createFailedButton.gameObject.SetActive(true);
        }
    }

    IEnumerator JoinRoomRequest(string code)
    {
        UnityWebRequest req = UnityWebRequest.Get($"https://kartoonrider-production.up.railway.app/rooms/join/code={code}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("참가 성공");
            popupJoinInput.SetActive(false);
            popupBackgroundOverlay.SetActive(false);
            Player.SetActive(true); // 플레이어 오브젝트 활성화    
            roomStatusUI.ShowAfterJoinPanel();     
        }
        else
        {
            Debug.LogError("참가 실패: " + req.error);
            popupJoinFailedWarn.SetActive(true);
        }
    }

    void Start(){
        popupJoinInput.SetActive(false);
        popupBackgroundOverlay.SetActive(false);  
        popupJoinFailedWarn.SetActive(false);
        popupCreateResult.SetActive(false);
        createFailedButton.gameObject.SetActive(false);
    }
}
