using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoginSceneScript : MonoBehaviour
{
    public TMP_InputField loginIdInput;
    public TMP_InputField loginPwInput;

    public TMP_InputField signUpIdInput;
    public TMP_InputField signUpPwInput;
    public TMP_InputField nicknameInput;

    public GameObject errorSet; // ErrorSet 오브젝트 연결
    public GameObject signUpError;
    
    public GameObject LoginPanel;
    public GameObject SignUpPanel;

    public Button loginButton;
    public Button gotoSignupButton;
    public Button signUpButton;
    public Button gotoLoginButton;


    void Start()
    {
        LoginPanel.SetActive(true);
        SignUpPanel.SetActive(false);
        errorSet.SetActive(false);
        signUpError.SetActive(false);
    }


    public void OnLoginButtonClicked()
    {
        string id = loginIdInput.text;
        string pw = loginPwInput.text;

        StartCoroutine(LoginRequest(id, pw));
    }

    public void OnSignUpButtonClicked()
    {
        string id = signUpIdInput.text;
        string pw = signUpPwInput.text;
        string name = nicknameInput.text;

        StartCoroutine(SignupRequest(id, pw, name));
    }

    public void OnGotoSignUpButton(){
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(true);
    }

    public void OnGotoLoginButton(){
        LoginPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }

    IEnumerator LoginRequest(string id, string pw)
    {
        string url = "https://kartoonrider-production-b878.up.railway.app/auth/login";
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
            SaveLoginToken(www.downloadHandler.text);
            if (errorSet != null)
                errorSet.SetActive(false); // 성공 시 에러 배경 숨김
            LoadIntroScene();
        }
        else
        {
            if (www.responseCode == 401 || www.responseCode == 404)
            {
                if (errorSet != null)
                    errorSet.SetActive(true); // 에러 시 배경+텍스트 표시
                Debug.LogError("The account does not exist or the password is incorrect.");
            }
            else
            {
                if (errorSet != null)
                    errorSet.SetActive(true); // 기타 에러도 배경+텍스트 표시
                Debug.Log("로그인 실패: " + www.error + " / " + www.downloadHandler.text);
            }
        }
    }

    IEnumerator SignupRequest(string id, string pw, string name)
    {
        string url = "https://kartoonrider-production-b878.up.railway.app/auth/register";
        string json = JsonUtility.ToJson(new SignupRequestData(id, pw, name));
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("회원가입 성공: " + www.downloadHandler.text);
            if (signUpError != null)
                signUpError.SetActive(false); // 성공 시 에러 배경 숨김
            // 회원가입 성공 후 로그인 패널로 전환 등 추가 동작 가능
            LoginPanel.SetActive(true); SignUpPanel.SetActive(false);
        }
        else
        {
            if (signUpError != null)
                signUpError.SetActive(true); // 에러 시 배경+텍스트 표시
            Debug.LogError("회원가입 실패: " + www.error + " / " + www.downloadHandler.text);
        }
    }

    void SaveLoginToken(string responseJson)
    {
        // 응답 JSON을 파싱해서 access_token만 저장
        LoginResponseData data = JsonUtility.FromJson<LoginResponseData>(responseJson);
        if (!string.IsNullOrEmpty(data.access_token))
        {
            PlayerPrefs.SetString("AuthToken", data.access_token);
            PlayerPrefs.Save();
            Debug.Log("로그인 토큰이 저장되었습니다: " + data.access_token);

            UserManager.Instance.SetUserId(data.user_id);

        }
        else
        {
            Debug.LogError("access_token이 응답에 없습니다.");
        }
    }

    void LoadIntroScene()
    {
        Debug.Log("IntroScene으로 전환합니다.");
        SceneManager.LoadScene("IntroScene");
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

    [System.Serializable]
    public class LoginResponseData
    {
        public string access_token;
        public string token_type;
        public int user_id;
    }

    [System.Serializable]
    public class SignupRequestData
    {
        public string login_id;
        public string password;
        public string nickname;
        public SignupRequestData(string id, string pw, string name)
        {
            login_id = id;
            password = pw;
            nickname = name;
        }
    }

}
