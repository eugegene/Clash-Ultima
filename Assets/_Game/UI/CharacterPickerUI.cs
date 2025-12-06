using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterPickerUI : MonoBehaviour
{
    [Header("Menu Controller")]
    public MainMenuController menuController;

    [Header("Data")]
    public CharacterDatabase database;

    [Header("UI References")]
    public Transform verseContainer;
    public Transform characterContainer;
    public TMP_InputField searchInput;
    public Toggle showNamesToggle;
    
    [Header("Prefabs")]
    public GameObject verseButtonPrefab;
    public GameObject characterItemPrefab;

    private List<CharacterGridItem> _spawnedCharacters = new List<CharacterGridItem>();
    private string _currentVerse = "";

    //void Start()
    //{
    //    PopulateVerses();
        
    //    // Listeners
    //    searchInput.onValueChanged.AddListener(OnSearch);
    //    showNamesToggle.onValueChanged.AddListener(OnToggleNames);
        
    //    // Select first verse by default
    //    var verses = database.GetVerses();
    //    if (verses.Count > 0) OnVerseSelected(verses[0]);
    //}

    // Change Start to OnEnable so it runs every time the menu opens
    void OnEnable()
    {
        Debug.Log("Character Picker Opened! Populating...");

        if (database == null)
        {
            Debug.LogError("DATABASE IS MISSING!");
            return;
        }

        PopulateVerses();
        
        // Setup Listeners (Check if already added to avoid duplicates)
        searchInput.onValueChanged.RemoveAllListeners();
        searchInput.onValueChanged.AddListener(OnSearch);
        
        showNamesToggle.onValueChanged.RemoveAllListeners();
        showNamesToggle.onValueChanged.AddListener(OnToggleNames);
        
        // Select first verse
        var verses = database.GetVerses();
        Debug.Log($"Found {verses.Count} verses in DB.");
        
        if (verses.Count > 0) OnVerseSelected(verses[0]);
    }

    private void PopulateVerses()
    {
        // Clear existing
        foreach (Transform child in verseContainer) Destroy(child.gameObject);

        // Spawn new
        foreach (string verse in database.GetVerses())
        {
            GameObject btn = Instantiate(verseButtonPrefab, verseContainer);
            btn.GetComponent<VerseButton>().Setup(verse, this);
        }
    }

    public void OnVerseSelected(string verse)
    {
        _currentVerse = verse;
        RefreshGrid(database.GetCharactersByVerse(verse));
    }

    public void OnSearch(string query)
    {
        query = query.ToLower();
        List<UnitDefinition> filtered = new List<UnitDefinition>();

        // If searching, search ALL characters, ignore verse
        if (!string.IsNullOrEmpty(query))
        {
            filtered = database.allCharacters
                .Where(c => c.unitName.ToLower().Contains(query))
                .ToList();
        }
        else
        {
            // If empty, return to current verse
            filtered = database.GetCharactersByVerse(_currentVerse);
        }

        RefreshGrid(filtered);
    }

    private void RefreshGrid(List<UnitDefinition> chars)
    {
        foreach (Transform child in characterContainer) Destroy(child.gameObject);
        _spawnedCharacters.Clear();

        foreach (var def in chars)
        {
            GameObject item = Instantiate(characterItemPrefab, characterContainer);
            var script = item.GetComponent<CharacterGridItem>();
            script.Setup(def, this, showNamesToggle.isOn);
            _spawnedCharacters.Add(script);
        }
    }

    private void OnToggleNames(bool show)
    {
        foreach (var item in _spawnedCharacters) item.SetNameVisibility(show);
    }

    //public void OnCharacterSelected(UnitDefinition def)
    //{
    //    Debug.Log($"Selected: {def.unitName}");
        
    //    // 1. Save to Session
    //    if (GameSession.Instance != null)
    //    {
    //        GameSession.Instance.SelectedCharacter = def;
    //    }

    //    // 2. Load Game (if we are in Main Menu)
    //    // If we are already in the Sandbox, we might want to just Respawn (TODO)
    //    if (SceneManager.GetActiveScene().name == "MainMenu")
    //    {
    //        SceneManager.LoadScene("SampleScene");
    //    }
    //    else
    //    {
    //        // We are in Sandbox -> Trigger Respawn logic here later
    //        // For now, reload scene to apply change
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //    }
    //}

    public void OnCharacterSelected(UnitDefinition def)
    {
        Debug.Log($"Selected: {def.unitName}");
        
        // 1. Save selection to the persistent GameSession
        if (GameSession.Instance != null)
        {
            GameSession.Instance.SelectedCharacter = def;
        }
        else
        {
            Debug.LogError("No GameSession found! Did you add it to the MainMenu scene?");
        }

        // 2. Load the Sandbox Scene
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClick_Back()
    {
        // Tell the controller to go back
        if (menuController != null)
        {
            menuController.ShowMainMenu();
        }
    }
}