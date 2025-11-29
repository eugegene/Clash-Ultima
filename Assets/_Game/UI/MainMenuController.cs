using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MainMenuController : MonoBehaviour
{
    // Name of your sandbox scene
    [SerializeField] private string sandboxSceneName = "SampleScene";

    public void OnClick_Sandbox()
    {
        // Load the gameplay scene
        SceneManager.LoadScene(sandboxSceneName);
    }

    public void OnClick_Exit()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}