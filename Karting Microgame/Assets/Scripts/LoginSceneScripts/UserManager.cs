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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUserId(int id)
    {
        UserId = id;
        Debug.Log($"✅ 로그인한 유저 ID 저장됨: {id}");
    }
}
