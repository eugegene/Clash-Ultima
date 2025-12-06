using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;       // The black overlay with buttons
    public GameObject pickerPanel;      // The Character Picker container

    private bool _isPaused = false;

    void Start()
    {
        // Ensure everything is hidden at start
        ResumeGame();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // If Picker is open, go back to Pause Menu
            if (pickerPanel != null && pickerPanel.activeSelf)
            {
                OpenPauseMenu();
            }
            // If Game is paused, Resume
            else if (_isPaused) 
            {
                ResumeGame();
            }
            // Otherwise, Pause
            else 
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f; // Freeze Time
        OpenPauseMenu();
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f; // Unfreeze Time
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (pickerPanel != null) pickerPanel.SetActive(false);
    }

    // --- Navigation Methods ---

    public void OpenPauseMenu()
    {
        pausePanel.SetActive(false);
        if (pickerPanel != null) 
        {
            pickerPanel.SetActive(true);
            
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(pickerPanel.GetComponent<RectTransform>());
        }
    }

    public void OpenCharacterPicker()
    {
        pausePanel.SetActive(false);
        if (pickerPanel != null) pickerPanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}