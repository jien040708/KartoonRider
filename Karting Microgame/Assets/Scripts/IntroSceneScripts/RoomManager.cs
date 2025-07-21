using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public GameObject popupCreateResult;
    public TMP_Text roomCodeText;

    public GameObject popupJoinInput;
    public GameObject popupJoinFailedWarn;
    public TMP_InputField joinCodeInput;

    public GameObject popupBackgroundOverlay;

    public void OnClickCreateRoom()
    {
        StartCoroutine(CreateRoomRequest());
    }

    public void OnClickJoinRoom()
    {
        popupJoinInput.SetActive(true);
        popupBackgroundOverlay.SetActive(true);
    }

    public void OnClickJoinConfirm()
    {
        string inputCode = joinCodeInput.text;
        StartCoroutine(JoinRoomRequest(inputCode));
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
            popupCreateResult.SetActive(true);
            popupBackgroundOverlay.SetActive(true);
        }
        else
        {
            Debug.LogError("방 생성 실패: " + req.error);
        }
    }

    IEnumerator JoinRoomRequest(string code)
    {
        UnityWebRequest req = UnityWebRequest.Get($"https://kartoonrider-production.up.railway.app/rooms/join/code={code}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("참가 성공");
            popupJoinInput.SetActive(true);
            popupBackgroundOverlay.SetActive(true);            
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
    }
}
