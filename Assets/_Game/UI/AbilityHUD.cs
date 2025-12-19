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
    private UnitStats _playerStats;
    
    // Active Slots
    private AbilitySlotUI _slotQ;
    private AbilitySlotUI _slotW;
    private AbilitySlotUI _slotE;
    private AbilitySlotUI _slotR;

    private struct PassiveTracker
    {
        public PassiveDefinition definition;
        public AbilitySlotUI ui;
    }
    private List<PassiveTracker> _passiveSlots = new List<PassiveTracker>();

    void Update()
    {
        // 1. Keep trying to find the player until successful
        if (_playerRef == null || _playerStats == null)
        {
            FindPlayer();
            return;
        }

        UpdateCooldowns();
        UpdatePassives();
    }

    private void FindPlayer()
    {
        UnitAbilityManager foundManager = FindObjectOfType<UnitAbilityManager>();
        
        if (foundManager != null)
        {
            _playerRef = foundManager;

            // --- FIX START: Search Parent and Children for Stats ---
            _playerStats = foundManager.GetComponent<UnitStats>();
            if (_playerStats == null) _playerStats = foundManager.GetComponentInParent<UnitStats>();
            if (_playerStats == null) _playerStats = foundManager.GetComponentInChildren<UnitStats>();
            // --- FIX END ---

            if (_playerStats != null)
            {
                Debug.Log($"[HUD] SUCCESS: Player Linked! Manager on '{_playerRef.name}', Stats on '{_playerStats.name}'");
                SetupSlots();
            }
            else
            {
                // If we see this, we know exactly why the overlay is missing
                Debug.LogWarning($"[HUD] Found AbilityManager on '{_playerRef.name}' but CANNOT find UnitStats! Passives will not update.");
            }
        }
    }

    private void SetupSlots()
    {
        if (slotContainer == null) return;
        
        Transform effectivePassiveContainer = (passiveContainer != null) ? passiveContainer : slotContainer;

        // Clear Old Slots
        foreach (Transform child in slotContainer) Destroy(child.gameObject);
        if (effectivePassiveContainer != slotContainer)
        {
            foreach (Transform child in effectivePassiveContainer) Destroy(child.gameObject);
        }
        _passiveSlots.Clear();

        // 1. Create Passives
        // NOTE: Ensure your UnitAbilityManager has the 'public List<PassiveDefinition> passives' field!
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
                        _passiveSlots.Add(new PassiveTracker { definition = passiveDef, ui = ui });
                    }
                }
            }
        }

        // 2. Create Active Abilities
        if (_playerRef.abilityQ != null) _slotQ = CreateAndInit(_playerRef.abilityQ, "Q");
        if (_playerRef.abilityW != null) _slotW = CreateAndInit(_playerRef.abilityW, "W");
        if (_playerRef.abilityE != null) _slotE = CreateAndInit(_playerRef.abilityE, "E");
        if (_playerRef.abilityR != null) _slotR = CreateAndInit(_playerRef.abilityR, "R");
    }

    private AbilitySlotUI CreateSlotObj(GameObject prefab, Transform container)
    {
        if (prefab == null) return null;
        GameObject obj = Instantiate(prefab, container);
        return obj.GetComponent<AbilitySlotUI>();
    }

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

    private void UpdatePassives()
    {
        if (_playerStats == null) return;

        // 1. Check Dedicated Passive Slots (The small icons)
        foreach (var tracker in _passiveSlots)
        {
            bool conditionMet = tracker.definition.IsConditionMet(_playerStats);
            tracker.ui.SetPassiveState(conditionMet);
        }

        // 2. Check Active Slots (Q, W, E, R) 
        // If an active slot holds an Ability_PassiveSlot, we must check its condition too.
        CheckActiveSlotForPassive(_playerRef.abilityQ, _slotQ);
        CheckActiveSlotForPassive(_playerRef.abilityW, _slotW);
        CheckActiveSlotForPassive(_playerRef.abilityE, _slotE);
        CheckActiveSlotForPassive(_playerRef.abilityR, _slotR);
    }

    private void CheckActiveSlotForPassive(AbilityDefinition def, AbilitySlotUI ui)
    {
        // Safety checks
        if (def == null || ui == null) return;

        // Check if the ability in this slot is actually a 'Ability_PassiveSlot' adapter
        if (def is Ability_PassiveSlot passiveAdapter)
        {
            if (passiveAdapter.passiveToEquip != null)
            {
                // Check the condition of the inner passive
                bool conditionMet = passiveAdapter.passiveToEquip.IsConditionMet(_playerStats);
                
                // Apply the overlay to the active slot UI
                ui.SetPassiveState(conditionMet);
            }
        }
    }
}