using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class AbilitySlotUI : MonoBehaviour
{
    // We removed 'public' so they don't show up in Inspector to confuse you.
    // The script will find them automatically.
    private Image _iconImage;
    private Image _cooldownOverlay;
    private TMP_Text _cooldownText;
    private TMP_Text _keyText;

    private float _cooldownDuration;

    void Awake()
    {
        // --- AUTO-WIRING SECTION ---
        // This looks through all children to find the components we need.
        
        // 1. Find the Icon (It's usually the first Image that ISN'T the background)
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach (var img in allImages)
        {
            // Assuming the background is on the root, we want the child image
            if (img.gameObject != this.gameObject && img.type != Image.Type.Filled) 
            {
                _iconImage = img;
                break;
            }
        }

        // 2. Find the Cooldown Overlay (The one set to "Filled")
        foreach (var img in allImages)
        {
            if (img.type == Image.Type.Filled)
            {
                _cooldownOverlay = img;
                break;
            }
        }

        // 3. Find the Texts
        TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>(true);
        foreach (var txt in allTexts)
        {
            // If the text is short (like "Q"), it's the Key. If it's empty or numbers, it's the Timer.
            if (txt.text.Length <= 1 && !char.IsDigit(txt.text.FirstOrDefault()))
                _keyText = txt;
            else
                _cooldownText = txt;
        }

        // Debugging to verify it worked
        if (_iconImage == null) Debug.LogError($"{name}: Could not auto-find an Icon Image!");
        if (_cooldownOverlay == null) Debug.LogWarning($"{name}: Could not auto-find a Filled Image for Cooldowns.");
    }

    public void Initialize(AbilityDefinition ability, string key)
    {
        // Safety: If auto-wiring failed, don't crash
        if (_iconImage == null) return;

        if (ability != null)
        {
            _iconImage.sprite = ability.icon;
            _iconImage.enabled = (ability.icon != null);
            _iconImage.color = Color.white; // Ensure full visibility
            _cooldownDuration = ability.cooldown;
        }
        else
        {
            _iconImage.enabled = false;
            _cooldownDuration = 0;
        }

        if (_keyText != null) _keyText.text = key;
        if (_cooldownOverlay != null) _cooldownOverlay.fillAmount = 0;
        if (_cooldownText != null) _cooldownText.text = "";
    }

    public void UpdateCooldown(float currentCooldown)
    {
        if (_cooldownDuration <= 0) return;

        if (currentCooldown > 0)
        {
            float fill = Mathf.Clamp01(currentCooldown / _cooldownDuration);
            if (_cooldownOverlay != null) _cooldownOverlay.fillAmount = fill;
            
            if (_cooldownText != null)
            {
                if (currentCooldown < 10f)
                    _cooldownText.text = currentCooldown.ToString("F1");
                else
                    _cooldownText.text = Mathf.CeilToInt(currentCooldown).ToString();
            }
        }
        else
        {
            if (_cooldownOverlay != null) _cooldownOverlay.fillAmount = 0;
            if (_cooldownText != null) _cooldownText.text = "";
        }
    }
    
    // Helper for checking digits
    private char FirstOrDefault(string s) => string.IsNullOrEmpty(s) ? ' ' : s[0];
    private bool IsDigit(char c) => c >= '0' && c <= '9';
}