using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour
{
    [Header("UI References (Drag & Drop in Inspector)")]
    public Image iconImage;
    public Image cooldownOverlay;
    public TMP_Text cooldownText;
    public TMP_Text keyText;

    private float _cooldownDuration;

    // --- 1. Initialize for Active Abilities (Q, W, E, R) ---
    public void Initialize(AbilityDefinition ability, string key)
    {
        // Set Key Text
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
        else
        {
            // Empty Slot
            if (iconImage != null) iconImage.enabled = false;
        }

        // Reset Cooldown Visuals
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
        if (cooldownText != null) cooldownText.text = "";
    }

    // --- 2. Initialize for Passives (THE FIX) ---
    public void Initialize(PassiveDefinition passive)
    {
        // Passives don't have keys or timers, so clear them
        if (keyText != null) keyText.text = "";
        if (cooldownText != null) cooldownText.text = "";
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;

        // Set Icon
        if (passive != null && iconImage != null)
        {
            iconImage.sprite = passive.icon;
            iconImage.enabled = (passive.icon != null);
        }
    }

    public void UpdateCooldown(float currentCooldown)
    {
        // If it's a passive or no cooldown, do nothing
        if (_cooldownDuration <= 0) return;

        if (currentCooldown > 0)
        {
            float fill = Mathf.Clamp01(currentCooldown / _cooldownDuration);
            
            if (cooldownOverlay != null) 
                cooldownOverlay.fillAmount = fill;
            
            if (cooldownText != null)
            {
                if (currentCooldown < 10f)
                    cooldownText.text = currentCooldown.ToString("F1");
                else
                    cooldownText.text = Mathf.CeilToInt(currentCooldown).ToString();
            }
        }
        else
        {
            // Cooldown Ready
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.text = "";
        }
    }

    public void SetPassiveState(bool isConditionMet)
    {
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = isConditionMet ? 0f : 1f;
        }
        else
        {
            Debug.LogError($"[UI] CRITICAL: 'Cooldown Overlay' is MISSING on {gameObject.name}! Drag the image in the Inspector.");
        }
    }
}