using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityHUD : MonoBehaviour
{
    [Header("References")]
    public GameObject slotPrefab;
    public Transform slotContainer;

    // Internal State
    private UnitAbilityManager _playerRef;
    
    // We store specific references so we know which timer updates which slot
    private AbilitySlotUI _slotQ;
    private AbilitySlotUI _slotW;
    private AbilitySlotUI _slotE;
    private AbilitySlotUI _slotR;

    void Update()
    {
        if (_playerRef == null)
        {
            FindPlayer();
            return;
        }

        UpdateCooldowns();
    }

    private void FindPlayer()
    {
        UnitAbilityManager found = FindObjectOfType<UnitAbilityManager>();
        if (found != null)
        {
            _playerRef = found;
            SetupSlots();
        }
    }

    private void SetupSlots()
    {
        // Clear old slots
        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        // Only create slots if the ability actually exists (Skip nulls)
        if (_playerRef.abilityQ != null) _slotQ = CreateSlot(_playerRef.abilityQ, "Q");
        if (_playerRef.abilityW != null) _slotW = CreateSlot(_playerRef.abilityW, "W");
        if (_playerRef.abilityE != null) _slotE = CreateSlot(_playerRef.abilityE, "E");
        if (_playerRef.abilityR != null) _slotR = CreateSlot(_playerRef.abilityR, "R");
    }

    private AbilitySlotUI CreateSlot(AbilityDefinition ability, string key)
    {
        if (slotPrefab == null) return null;

        GameObject newObj = Instantiate(slotPrefab, slotContainer);
        AbilitySlotUI uiScript = newObj.GetComponent<AbilitySlotUI>();

        if (uiScript != null)
        {
            uiScript.Initialize(ability, key);
        }
        return uiScript;
    }

    private void UpdateCooldowns()
    {
        // Only update the slots that were created
        if (_slotQ != null) _slotQ.UpdateCooldown(_playerRef.cooldownQ);
        if (_slotW != null) _slotW.UpdateCooldown(_playerRef.cooldownW);
        if (_slotE != null) _slotE.UpdateCooldown(_playerRef.cooldownE);
        if (_slotR != null) _slotR.UpdateCooldown(_playerRef.cooldownR);
    }
}