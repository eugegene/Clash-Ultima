using UnityEngine;
using System.Collections;

// This component is added to the victim to handle the Root effect lifespan
public class Buff_CrimsonBinding : MonoBehaviour
{
    private float _duration;
    private UnitStats _stats;
    private StatModifier _rootModifier;

    public void Initialize(float duration)
    {
        _duration = duration;
        _stats = GetComponent<UnitStats>();

        if (_stats != null)
        {
            // Create a modifier that Multiplies MoveSpeed by 0 (Effective Root)
            // StatModType.PercentMult with value 0f results in 0 total speed.
            _rootModifier = new StatModifier(0f, StatModType.PercentMult, this);
            
            _stats.MoveSpeed.AddModifier(_rootModifier);
            
            // Start the timer to remove the root
            StartCoroutine(DurationRoutine());
        }
        else
        {
            // If target has no stats (e.g. a crate), just remove this immediately
            Destroy(this);
        }
    }

    private IEnumerator DurationRoutine()
    {
        yield return new WaitForSeconds(_duration);
        RemoveEffect();
    }

    private void RemoveEffect()
    {
        if (_stats != null && _rootModifier != null)
        {
            _stats.MoveSpeed.RemoveModifier(_rootModifier);
        }
        Destroy(this);
    }

    // Safety check: ensure stats are restored if the object is destroyed unexpectedly
    private void OnDestroy()
    {
        if (_stats != null && _rootModifier != null)
        {
            _stats.MoveSpeed.RemoveModifier(_rootModifier);
        }
    }
}