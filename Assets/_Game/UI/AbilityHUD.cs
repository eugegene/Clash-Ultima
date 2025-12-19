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
    private UnitStats _playerStats; // Cache stats for efficiency
    
    // Active Slots
    private AbilitySlotUI _slotQ;
    private AbilitySlotUI _slotW;
    private AbilitySlotUI _slotE;
    private AbilitySlotUI _slotR;

    // --- NEW: Track Passives to update them ---
    private struct PassiveTracker
    {
        public PassiveDefinition definition;
        public AbilitySlotUI ui;
    }
    private List<PassiveTracker> _passiveSlots = new List<PassiveTracker>();

    void Update()
    {
        if (_playerRef == null)
        {
            FindPlayer();
            return;
        }

        UpdateCooldowns();
        UpdatePassives(); // <--- Check conditions every frame
    }

    private void FindPlayer()
    {
        UnitAbilityManager found = FindObjectOfType<UnitAbilityManager>();
        if (found != null)
        {
            _playerRef = found;
            _playerStats = found.GetComponent<UnitStats>(); // Get Stats once
            SetupSlots();
        }
    }

    private void SetupSlots()
    {
        if (slotContainer == null) return;

        // 1. Determine Container
        Transform effectivePassiveContainer = (passiveContainer != null) ? passiveContainer : slotContainer;

        // 2. Clear Lists
        foreach (Transform child in slotContainer) Destroy(child.gameObject);
        if (effectivePassiveContainer != slotContainer)
        {
            foreach (Transform child in effectivePassiveContainer) Destroy(child.gameObject);
        }
        _passiveSlots.Clear();

        // 3. Create Passives
        if (_playerRef.passives != null)
        {
            foreach (var passiveDef in _playerRef.passives)
            {
                if (passiveDef != null)
                {
                    GameObject prefabToUse = (passiveSlotPrefab != null) ? passiveSlotPrefab : slotPrefab;
                    AbilitySlotUI ui = CreateSlotObj(prefabToUse, effectivePassiveContainer);
                    
                    if (ui != null)
                    {
                        ui.Initialize(passiveDef);
                        // Add to list so we can update it later
                        _passiveSlots.Add(new PassiveTracker { definition = passiveDef, ui = ui });
                    }
                }
            }
        }

        // 4. Create Active Abilities
        if (_playerRef.abilityQ != null) _slotQ = CreateAndInit(_playerRef.abilityQ, "Q");
        if (_playerRef.abilityW != null) _slotW = CreateAndInit(_playerRef.abilityW, "W");
        if (_playerRef.abilityE != null) _slotE = CreateAndInit(_playerRef.abilityE, "E");
        if (_playerRef.abilityR != null) _slotR = CreateAndInit(_playerRef.abilityR, "R");
    }

    // Helper to instantiate and return the script
    private AbilitySlotUI CreateSlotObj(GameObject prefab, Transform container)
    {
        if (prefab == null) return null;
        GameObject obj = Instantiate(prefab, container);
        return obj.GetComponent<AbilitySlotUI>();
    }

    // Helper for Actives
    private AbilitySlotUI CreateAndInit(AbilityDefinition def, string key)
    {
        AbilitySlotUI ui = CreateSlotObj(slotPrefab, slotContainer);
        if (ui != null) ui.Initialize(def, key);
        return ui;
    }

    private void UpdateCooldowns()
    {
        if (_slotQ != null) _slotQ.UpdateCooldown(_playerRef.cooldownQ);
        if (_slotW != null) _slotW.UpdateCooldown(_playerRef.cooldownW);
        if (_slotE != null) _slotE.UpdateCooldown(_playerRef.cooldownE);
        if (_slotR != null) _slotR.UpdateCooldown(_playerRef.cooldownR);
    }

    // --- NEW: Update Passives Loop ---
    private void UpdatePassives()
    {
        if (_playerStats == null) return;

        foreach (var tracker in _passiveSlots)
        {
            // Ask the definition: "Are we happy?"
            bool conditionMet = tracker.definition.IsConditionMet(_playerStats);
            
            // Tell the UI: "Show Overlay if unhappy"
            tracker.ui.SetPassiveState(conditionMet);
        }
    }
}