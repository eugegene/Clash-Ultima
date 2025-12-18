using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour
{
    [Header("UI References (Drag these in Prefab!)")]
    public Image iconImage;
    public Image cooldownOverlay;
    public TMP_Text cooldownText;
    public TMP_Text keyText;

    private float _cooldownDuration;

    void Awake()
    {
        // --- FALLBACK AUTO-WIRING ---
        // If you forgot to assign them in the Inspector, we try to find them by name.
        
        if (iconImage == null) iconImage = FindChild<Image>("Icon");
        if (cooldownOverlay == null) cooldownOverlay = FindChild<Image>("Cooldown", "Fill", "Overlay");
        if (keyText == null) keyText = FindChild<TMP_Text>("Key", "HotKey");
        if (cooldownText == null) cooldownText = FindChild<TMP_Text>("Timer", "Cooldown", "Text");

        // --- FORCE FIXES ---
        // Ensure the Cooldown Overlay is actually set to "Filled" mode so it can animate
        if (cooldownOverlay != null && cooldownOverlay.type != Image.Type.Filled)
        {
            cooldownOverlay.type = Image.Type.Filled;
            cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
            cooldownOverlay.fillAmount = 0; // Start empty
        }

        // Hide the timer text by default
        if (cooldownText != null) cooldownText.text = "";
    }

    public void Initialize(AbilityDefinition ability, string key)
    {
        // Set the Key (Q, W, E, R)
        if (keyText != null) keyText.text = key;

        if (ability != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = ability.icon;
                iconImage.enabled = (ability.icon != null);
            }
            _cooldownDuration = ability.cooldown;
        }
        
        // Reset Cooldown State
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
        if (cooldownText != null) cooldownText.text = "";
    }

    public void UpdateCooldown(float currentCooldown)
    {
        if (_cooldownDuration <= 0) return;

        if (currentCooldown > 0)
        {
            float fill = Mathf.Clamp01(currentCooldown / _cooldownDuration);
            
            if (cooldownOverlay != null) 
                cooldownOverlay.fillAmount = fill;
            
            if (cooldownText != null)
            {
                // Show "3.5" for small numbers, "4" for big ones
                if (currentCooldown < 10f)
                    cooldownText.text = currentCooldown.ToString("F1");
                else
                    cooldownText.text = Mathf.CeilToInt(currentCooldown).ToString();
            }
        }
        else
        {
            // Cooldown Finished
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.text = "";
        }
    }

    // Helper to find components by potential names
    private T FindChild<T>(params string[] potentialNames) where T : Component
    {
        T[] allComponents = GetComponentsInChildren<T>(true);
        foreach (var comp in allComponents)
        {
            // Check if the GameObject name contains any of our keywords
            foreach (var nameKey in potentialNames)
            {
                if (comp.name.IndexOf(nameKey, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return comp;
            }
        }
        return null;
    }
}