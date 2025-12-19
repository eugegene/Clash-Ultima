using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityHUD : MonoBehaviour
{
    [Header("Active Abilities")]
    public GameObject slotPrefab;
    public Transform slotContainer;

    [Header("Passive Abilities")]
    public GameObject passiveSlotPrefab;
    public Transform passiveContainer;

    private UnitAbilityManager _playerRef;
    
    // Active Slots References
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
        if (slotContainer == null) return;

        // --- SMART CONTAINER LOGIC ---
        // 1. Determine where passives go. (If null, default to the main slot container)
        Transform effectivePassiveContainer = (passiveContainer != null) ? passiveContainer : slotContainer;

        // 2. Clear Main Container
        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        // 3. Clear Passive Container 
        // (Only if it is a DIFFERENT object. If it's the same, we just cleared it above!)
        if (effectivePassiveContainer != slotContainer)
        {
            foreach (Transform child in effectivePassiveContainer) Destroy(child.gameObject);
        }

        // --- GENERATION ORDER ---
        // We generate Passives FIRST. 
        // If they share a container, this puts Passives on the LEFT.

        // 4. Create Passives
        if (_playerRef.passives != null)
        {
            foreach (var passiveDef in _playerRef.passives)
            {
                if (passiveDef != null)
                {
                    // Use passiveSlotPrefab if available, otherwise fallback to standard slot
                    GameObject prefabToUse = (passiveSlotPrefab != null) ? passiveSlotPrefab : slotPrefab;
                    CreatePassiveSlot(passiveDef, effectivePassiveContainer, prefabToUse);
                }
            }
        }

        // 5. Create Active Abilities (Q, W, E, R)
        if (_playerRef.abilityQ != null) _slotQ = CreateSlot(_playerRef.abilityQ, "Q", slotContainer, slotPrefab);
        if (_playerRef.abilityW != null) _slotW = CreateSlot(_playerRef.abilityW, "W", slotContainer, slotPrefab);
        if (_playerRef.abilityE != null) _slotE = CreateSlot(_playerRef.abilityE, "E", slotContainer, slotPrefab);
        if (_playerRef.abilityR != null) _slotR = CreateSlot(_playerRef.abilityR, "R", slotContainer, slotPrefab);
    }

    private AbilitySlotUI CreateSlot(AbilityDefinition ability, string key, Transform container, GameObject prefab)
    {
        if (prefab == null) return null;
        GameObject newObj = Instantiate(prefab, container);
        AbilitySlotUI uiScript = newObj.GetComponent<AbilitySlotUI>();
        if (uiScript != null) uiScript.Initialize(ability, key);
        return uiScript;
    }

    private void CreatePassiveSlot(PassiveDefinition passive, Transform container, GameObject prefab)
    {
        if (prefab == null) return;
        GameObject newObj = Instantiate(prefab, container);
        AbilitySlotUI uiScript = newObj.GetComponent<AbilitySlotUI>();
        if (uiScript != null) uiScript.Initialize(passive);
    }

    private void UpdateCooldowns()
    {
        if (_slotQ != null) _slotQ.UpdateCooldown(_playerRef.cooldownQ);
        if (_slotW != null) _slotW.UpdateCooldown(_playerRef.cooldownW);
        if (_slotE != null) _slotE.UpdateCooldown(_playerRef.cooldownE);
        if (_slotR != null) _slotR.UpdateCooldown(_playerRef.cooldownR);
    }
}