using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class LoginSceneScript : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public Button loginButton;

    void Start()
    {
        // InputField 이벤트 리스너 추가
        if (idInput != null)
            idInput.onValueChanged.AddListener(OnIdInputValueChanged);
        if (pwInput != null)
            pwInput.onValueChanged.AddListener(OnPwInputValueChanged);
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    // ID 입력값 변경 시 호출되는 함수
    void OnIdInputValueChanged(string value)
    {
        Debug.Log("ID 입력: " + value);
    }

    // PW 입력값 변경 시 호출되는 함수
    void OnPwInputValueChanged(string value)
    {
        Debug.Log("PW 입력: " + value);
    }

    void OnLoginButtonClicked()
    {
        string id = idInput.text;
        string pw = pwInput.text;
        StartCoroutine(LoginRequest(id, pw));
    }

    IEnumerator LoginRequest(string id, string pw)
    {
        string url = "https://kartoonrider-production.up.railway.app/auth/login";
        string json = JsonUtility.ToJson(new LoginRequestData(id, pw));

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("로그인 성공: " + www.downloadHandler.text);
            // JWT 토큰 파싱 및 저장, 씬 이동 등 추가 처리 가능
        }
        else
        {
            Debug.Log("로그인 실패: " + www.error + " / " + www.downloadHandler.text);
            // 실패 처리
        }
    }

    [System.Serializable]
    public class LoginRequestData
    {
        public string login_id;
        public string password;
        public LoginRequestData(string id, string pw)
        {
            login_id = id;
            password = pw;
        }
    }

    // OnDestroy에서 이벤트 리스너 제거
    void OnDestroy()
    {
        if (idInput != null)
            idInput.onValueChanged.RemoveListener(OnIdInputValueChanged);
        if (pwInput != null)
            pwInput.onValueChanged.RemoveListener(OnPwInputValueChanged);
        if (loginButton != null)
            loginButton.onClick.RemoveListener(OnLoginButtonClicked);
    }
}
