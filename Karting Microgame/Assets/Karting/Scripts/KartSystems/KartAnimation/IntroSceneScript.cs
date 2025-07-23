using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroSceneScript : MonoBehaviour
{
    public SkinPreviewUpdater previewUpdater;
    public GameObject Nami;
    public GameObject Sasuke;

    public GameObject Sahur;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (SkinManager.Instance.selectedCharacterPrefab == null)
        // {
        //     // 에러 방지용 기본값 세팅
        //     SkinManager.Instance.SetSkins(Nami);
        //     previewUpdater.ReplacePlayerIdle(Nami);
        // }
    }

    public void OnNamiClicked()
    {
        GameObject chosenCharacter = Nami;

        SkinManager.Instance.SetSkins(chosenCharacter);
        previewUpdater.ReplacePlayerIdle(chosenCharacter);
    }

    public void OnSasukeClicked()
    {
        GameObject chosenCharacter = Sasuke;

        SkinManager.Instance.SetSkins(chosenCharacter);
        previewUpdater.ReplacePlayerIdle(chosenCharacter);
    }

    public void OnSahurClicked()
    {
        GameObject chosenCharacter = Sahur;

        SkinManager.Instance.SetSkins(chosenCharacter);
        previewUpdater.ReplacePlayerIdle(chosenCharacter);
    }

    public void OnClickMainScene()
    {
        // 게임 씬으로 넘어가기 >> 씬 넘어가도 저장 되어있는 거 확인하고 나중에 삭제 필요
        SceneManager.LoadScene("MainSceneEdited");
    }
    void Update()
    {

    }
}
