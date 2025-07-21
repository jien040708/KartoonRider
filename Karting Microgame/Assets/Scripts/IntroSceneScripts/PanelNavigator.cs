using UnityEngine;

public class PanelNavigator : MonoBehaviour
{
    public GameObject characterPanel;
    public GameObject carPanel;

    public void ShowCharacter()
    {
        ShowOnly(characterPanel);
    }

    public void ShowCar()
    {
        ShowOnly(carPanel);
    }

    private void ShowOnly(GameObject target)
    {
        characterPanel.SetActive(false);
        carPanel.SetActive(false);
        target.SetActive(true);
    }

    void Start()
    {
        ShowOnly(characterPanel);
    }
}
