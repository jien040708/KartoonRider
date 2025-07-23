using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Analytics;
using System;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
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
    public override void OnJoinedRoom()
    {
        Debug.Log("✅ 방 참가 성공!");
        // 여기에 성공했을 때 UI 띄우거나 게임 시작하면 됨
        if(popupJoinFailedWarn.activeSelf)
        {
            popupJoinFailedWarn.SetActive(false);
        }
        roomStatusUI.ShowAfterJoinPanel();
        
        // PhotonRoomManager에 방 참가 알림
        if (PhotonRoomManager.Instance != null)
        {
            Debug.Log("PhotonRoomManager에 방 참가 알림");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ 방 참가 실패: " + message);
        popupJoinFailedWarn.SetActive(true);
        // 여기서 실패 UI 띄우면 됨
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"❌ 방 생성 실패! 이유: {message} (코드: {returnCode})");
        createFailedButton.gameObject.SetActive(true); 
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"새 플레이어 입장: {newPlayer.NickName}");
        roomStatusUI.UpdatePlayerCount();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        roomStatusUI.UpdatePlayerCount();
    }

    public void OnClickCreateRoom()
    {
        if (!PhotonInitializer.Instance.IsReady)
    {
        Debug.LogWarning("❌ Photon 서버가 아직 준비되지 않았습니다!");
        return;
    }
        string roomCode = Guid.NewGuid().ToString("N").Substring(0, 6);
        StartCoroutine(CreateRoomRequest(roomCode));
        Player.SetActive(false);
        popupCreateResult.SetActive(true);
        popupBackgroundOverlay.SetActive(true);

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
        createFailedButton.gameObject.SetActive(true);
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
        popupJoinInput.SetActive(false);
        popupBackgroundOverlay.SetActive(false);
        Player.SetActive(true);

        roomStatusUI.SetRoomCode(inputCode); //복귀2
        roomStatusUI.ShowAfterJoinPanel();
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

    IEnumerator CreateRoomRequest(string roomUUID)
    {
    //     if (!PhotonInitializer.Instance.IsReady)
    // {
    //     Debug.LogWarning("❌ Photon 서버가 아직 준비되지 않았습니다!");
    //     return;
    // }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2; // 테스트용으로 2명으로 변경

        PhotonNetwork.CreateRoom(roomUUID, options);
        roomCodeText.text = roomUUID;

        //방 생성 실패 로직
        // Player.SetActive(false);
        // createFailedButton.gameObject.SetActive(true);
        
        yield return null; // 코루틴이 제대로 작동하도록 yield return 추가
    }

    [System.Serializable]
    public class RoomCodeResponse
    {
        public string room_code;
    }



    IEnumerator JoinRoomRequest(string roomUUID)
    {
        PhotonNetwork.JoinRoom(roomUUID);
        yield return null; // 코루틴이 제대로 작동하도록 yield return 추가
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
