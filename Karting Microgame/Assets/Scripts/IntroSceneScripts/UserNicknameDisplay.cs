using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;



public class UserNicknameDisplay : MonoBehaviour
{
    [System.Serializable]
    public class NicknameOnly
    {
        public string nickname;
    }
    public TMP_Text nicknameText;
    private string accessToken;

    public void Start()
    {
        accessToken = PlayerPrefs.GetString("AuthToken", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 로그인 먼저 해주세요.");
            return;
        }
        StartCoroutine(GetNicknameFromServer());
    }

    IEnumerator GetNicknameFromServer()
    {
        string url = "https://kartoonrider-production-b878.up.railway.app/auth/me";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var json = www.downloadHandler.text;
            string nickname = JsonUtility.FromJson<NicknameOnly>(json).nickname;

            // nickname 값을 사용하도록 수정
            Debug.Log("닉네임 응답 JSON: " + json);
            Debug.Log("파싱된 닉네임: " + nickname);

            SetNickname(nickname);
        }
        else
        {
            Debug.LogError("유저 정보 불러오기 실패: " + www.error + " | " + www.downloadHandler.text);
        }
    }

    public void SetNickname(string name)
    {
        nicknameText.text = name;
    }
}