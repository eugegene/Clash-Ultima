using UnityEngine;
using System; // Required for Action

public class Buff_FlowingRedScale : MonoBehaviour
{
    public bool IsActive => _isActive;

    private UnitStats _stats;
    private UnitAttack _attack;

    // Configuration
    private float _speedBonus;
    private float _atkSpeedBonus;
    private float _critBonus;
    
    // State
    private float _lastCombatEventTime;
    private float _combatDurationAccumulator;
    private bool _isActive;

    // Modifiers
    private StatModifier _modMoveSpeed;
    private StatModifier _modAtkSpeed;
    private StatModifier _modCrit;

    // Event Cache (Required to safely unsubscribe later)
    private Action<UnitStats> _attackStartListener;

    // Constants
    private const float COMBAT_TOLERANCE_WINDOW = 3.0f; 
    private const float ACTIVATION_TIME_REQUIRED = 1.0f;
    //private const float ACTIVATION_TIME_REQUIRED = 10.0f;
    private const float DEACTIVATION_COOLDOWN = 20.0f;

    public void Initialize(UnitStats stats, float speed, float atkSpeed, float crit)
    {
        _stats = stats;
        _attack = stats.GetComponent<UnitAttack>();
        
        _speedBonus = speed;
        _atkSpeedBonus = atkSpeed;
        _critBonus = crit;

        // Reset state
        _lastCombatEventTime = -999f;
        _combatDurationAccumulator = 0f;

        // Subscribe to events
        if (_stats != null) 
            _stats.OnDamageTaken += OnCombatEvent;
        
        if (_attack != null)
        {
            // FIX: Store the lambda so we can remove it later
            // FIX: Use default(DamageMessage) because structs cannot be null
            _attackStartListener = (target) => OnCombatEvent(default(DamageMessage));
            
            _attack.OnAttackStarted += _attackStartListener;
            _attack.OnAfterDamageApplied += OnCombatEvent; 
        }
    }

    private void OnDisable()
    {
        // Clean up events
        if (_stats != null) 
            _stats.OnDamageTaken -= OnCombatEvent;
        
        if (_attack != null)
        {
            // Safely unsubscribe using the cached listener
            if (_attackStartListener != null)
                _attack.OnAttackStarted -= _attackStartListener;

            _attack.OnAfterDamageApplied -= OnCombatEvent;
        }
        
        RemoveBuffs();
    }

    // The method signature must match Action<DamageMessage>
    private void OnCombatEvent(DamageMessage msg)
    {
        // We ignore the content of the message; we just care that combat happened
        _lastCombatEventTime = Time.time;
    }

    void Update()
    {
        float timeSinceAction = Time.time - _lastCombatEventTime;

        if (_isActive)
        {
            // Deactivation Logic
            if (timeSinceAction >= DEACTIVATION_COOLDOWN)
            {
                RemoveBuffs();
                _combatDurationAccumulator = 0f;
                Debug.Log($"<color=cyan>{name}: Flowing Red Scale Deactivated (Out of Combat)</color>");
            }
        }
        else
        {
            // Activation Logic
            if (timeSinceAction <= COMBAT_TOLERANCE_WINDOW)
            {
                _combatDurationAccumulator += Time.deltaTime;
                
                if (_combatDurationAccumulator >= ACTIVATION_TIME_REQUIRED)
                {
                    ApplyBuffs();
                    Debug.Log($"<color=red>{name}: Flowing Red Scale ACTIVATED!</color>");
                }
            }
            else
            {
                _combatDurationAccumulator = 0f;
            }
        }
    }

    private void ApplyBuffs()
    {
        if (_isActive) return;
        _isActive = true;

        _modMoveSpeed = new StatModifier(_speedBonus, StatModType.PercentAdd, this);
        _modAtkSpeed = new StatModifier(_atkSpeedBonus, StatModType.PercentAdd, this);
        _modCrit = new StatModifier(_critBonus, StatModType.Flat, this);

        _stats.MoveSpeed.AddModifier(_modMoveSpeed);
        _stats.AttackSpeed.AddModifier(_modAtkSpeed);
        _stats.CritChance.AddModifier(_modCrit);
    }

    private void RemoveBuffs()
    {
        if (!_isActive) return;
        _isActive = false;

        _stats.MoveSpeed.RemoveModifier(_modMoveSpeed);
        _stats.AttackSpeed.RemoveModifier(_modAtkSpeed);
        _stats.CritChance.RemoveModifier(_modCrit);
    }
}