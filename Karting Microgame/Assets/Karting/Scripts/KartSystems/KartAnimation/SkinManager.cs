using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    //public GameObject selectedKartPrefab;
    public GameObject selectedCharacterPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 제거
        }
    }

    public void SetSkins(GameObject character)
    {
        //selectedKartPrefab = kart;
        selectedCharacterPrefab = character;
    }


}
