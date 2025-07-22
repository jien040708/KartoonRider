using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

// UserInfo 클래스를 여기에 정의하거나, 별도 파일로 관리할 수 있습니다.
[System.Serializable]
public class UserInfoWrapper
{
    public UserInfo user;
}

[System.Serializable]
public class UserInfo
{
    public int id;
    public string login_id;
    public string nickname;
    public int rating;
    public int coin;  // 백엔드에서 coin 필드 추가
    public string created_at;
}


public class CoinDisplay : MonoBehaviour
{
    public TMP_Text coinText;
    private string accessToken;

    public void Start()
    {
        accessToken = PlayerPrefs.GetString("AuthToken", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 로그인 먼저 해주세요.");
            return;
        }
        StartCoroutine(GetCoinFromServer());
    }

    IEnumerator GetCoinFromServer()
    {
        string url = "https://kartoonrider-production-b878.up.railway.app/auth/me";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var json = www.downloadHandler.text;
            UserInfo data = JsonUtility.FromJson<UserInfo>(json);
            // coin 값을 사용하도록 수정
            SetCoins(data.coin);
        }
        else
        {
            Debug.LogError("유저 정보 불러오기 실패: " + www.error + " | " + www.downloadHandler.text);
        }
    }

    public void SetCoins(int amount)
    {
        coinText.text = amount.ToString();
    }
}