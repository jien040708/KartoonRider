using UnityEngine;

public class MainSceneScript : MonoBehaviour
{
    public SkinPreviewUpdater skinPreviewUpdater;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skinPreviewUpdater.ReplaceMainPlayerIdle(SkinManager.Instance.selectedCharacterPrefab);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
