using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/Kamo/Blood Arrows")]
public class Ability_KamoBloodArrows : AbilityDefinition
{
    public override void OnCast(UnitStats caster, Vector3 point, UnitStats target)
    {
        // Find the KamoController on the caster
        KamoController kamo = caster.GetComponent<KamoController>();
        
        if (kamo != null)
        {
            kamo.ToggleBloodArrows();
        }
        else
        {
            Debug.LogError("Ability Casted, but KamoController not found on unit!");
        }
    }
}