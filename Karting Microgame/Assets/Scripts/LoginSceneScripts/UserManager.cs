using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;
    public int UserId { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
            Debug.Log("✅ UserManager 인스턴스 생성됨");

        }
        else
        {
            Debug.LogWarning("⚠️ 중복 UserManager 제거됨");

            Destroy(gameObject);

        }
    }

    public void SetUserId(int id)
    {
        UserId = id;
        Debug.Log($"✅ 로그인한 유저 ID 저장됨: {id}");
    }
}
