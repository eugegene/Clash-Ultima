using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuContainer;   // The "Sandbox" and "Exit" buttons
    public GameObject pickerContainer; // The Character Picker UI

    void Start()
    {
        // Ensure we start in the correct state
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        menuContainer.SetActive(true);
        pickerContainer.SetActive(false);
    }

    public void OnClick_Sandbox()
    {
        // Switch to Picker
        menuContainer.SetActive(false);
        pickerContainer.SetActive(true);
    }

    public void OnClick_Exit()
    {
        Application.Quit();
    }
}