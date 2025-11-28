using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float _baseValue; // Set in Inspector
    
    // We cache the value so we don't recalculate it every frame unless it changes
    private bool _isDirty = true; 
    private float _value;

    private readonly List<StatModifier> _modifiers = new List<StatModifier>();

    public float BaseValue
    {
        get { return _baseValue; }
        set { _baseValue = value; _isDirty = true; }
    }

    public float Value
    {
        get
        {
            if (_isDirty)
            {
                _value = CalculateFinalValue();
                _isDirty = false;
            }
            return _value;
        }
    }

    public Stat(float baseValue)
    {
        _baseValue = baseValue;
        _modifiers = new List<StatModifier>();
    }

    public void AddModifier(StatModifier mod)
    {
        _isDirty = true;
        _modifiers.Add(mod);
        _modifiers.Sort(CompareModifierOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        if (_modifiers.Remove(mod))
        {
            _isDirty = true;
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        bool removed = false;
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].Source == source)
            {
                _modifiers.RemoveAt(i);
                removed = true;
            }
        }
        if (removed) _isDirty = true;
        return removed;
    }

    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order) return -1;
        if (a.Order > b.Order) return 1;
        return 0;
    }

    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < _modifiers.Count; i++)
        {
            StatModifier mod = _modifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;
                // We deal with the sum AFTER the flat additions usually, 
                // or continuously depending on game math preference.
                // Here: Standard RPG math (Base + Flat) * (1 + Sum%)
                if (i + 1 >= _modifiers.Count || _modifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.Type == StatModType.PercentMult)
            {
                finalValue *= mod.Value;
            }
        }

        return (float)Math.Round(finalValue, 4); // Avoid floating point errors
    }
}