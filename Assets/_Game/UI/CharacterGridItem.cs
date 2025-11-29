using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterGridItem : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public Button button;

    private CharacterPickerUI _controller;
    private UnitDefinition _myDef;

    public void Setup(UnitDefinition def, CharacterPickerUI controller, bool showName)
    {
        _myDef = def;
        _controller = controller;

        iconImage.sprite = def.icon;
        nameText.text = def.unitName;
        nameText.gameObject.SetActive(showName);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _controller.OnCharacterSelected(_myDef));
    }

    public void SetNameVisibility(bool visible)
    {
        nameText.gameObject.SetActive(visible);
    }
}