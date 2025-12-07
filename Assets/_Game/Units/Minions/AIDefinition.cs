using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/AI Definition")]
public class AIDefinition : ScriptableObject
{
    [Header("Behavior")]
    public float aggroRange = 8f;     // How far can I see?
    public float attackInterval = 2f; // How often do I rethink my attack?
    public bool canUseAbilities = false; // Is this a Boss?

    [Header("Difficulty Modifiers (Multipliers)")]
    public float healthMod = 1.0f;    // 1.0 = Normal, 2.0 = Double HP
    public float damageMod = 1.0f;
    public float speedMod = 1.0f;
    //public float xpRewardMod = 1.0f;  // Harder enemies give more XP
}