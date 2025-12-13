using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/Kamo/Crimson Binding")]
public class Ability_CrimsonBinding : AbilityDefinition
{
    [Header("Crimson Binding Settings")]
    public float healthCost = 20f;
    public float duration = 2.0f;
    public GameObject visualPrefab; 

    public override void OnCast(UnitStats caster, Vector3 point, UnitStats target)
    {
        if (target == null) return;

        // --- NEW: Check Kamo Controller for Passive Restriction ---
        KamoController kamo = caster.GetComponent<KamoController>();
        if (kamo != null)
        {
            if (!kamo.CanUseBloodTechnique())
            {
                Debug.Log("<color=red>Cannot cast Crimson Binding: HP too low!</color>");
                return;
            }
        }
        // ----------------------------------------------------------

        if (caster.CurrentHealth < healthCost || caster.CurrentResource < manaCost)
        {
            Debug.Log("Not enough resources!");
            return;
        }

        caster.ModifyHealth(-healthCost);
        caster.ModifyResource(-manaCost);

        Buff_CrimsonBinding binding = target.gameObject.AddComponent<Buff_CrimsonBinding>();
        binding.Initialize(duration);

        if (visualPrefab != null)
        {
            GameObject vfx = Instantiate(visualPrefab, target.transform.position, Quaternion.identity);
            vfx.transform.SetParent(target.transform);
            Destroy(vfx, duration);
        }
    }
}