using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VerseButton : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Button button;
    
    private CharacterPickerUI _controller;
    private string _myVerse;

    public void Setup(string verseName, CharacterPickerUI controller)
    {
        _myVerse = verseName;
        _controller = controller;
        label.text = verseName;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _controller.OnVerseSelected(_myVerse));
    }
}