using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Needed for the Esc key

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuCanvas; // Drag the Panel/Canvas here

    [Header("Settings")]
    public string mainMenuScene = "MainMenu";

    private bool _isPaused = false;

    void Start()
    {
        // Ensure menu is hidden and time is running when game starts
        ResumeGame();
    }

    void Update()
    {
        // Check for ESC key
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f; // Freeze the game
    }

    public void ResumeGame()
    {
        _isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // Unfreeze the game
    }

    public void OnClick_Continue()
    {
        ResumeGame();
    }

    public void OnClick_PickCharacter()
    {
        // For now, we will just reload the scene to "reset"
        // Later, this will open a specific UI panel
        Debug.Log("Open Character Select Logic Here");
        
        // Optional: Reload scene to act as a "Reset"
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClick_MainMenu()
    {
        // IMPORTANT: Always reset time before leaving the scene!
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuScene);
    }
}