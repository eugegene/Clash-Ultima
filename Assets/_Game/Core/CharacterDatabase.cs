using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "MOBA/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<UnitDefinition> allCharacters;

    // Helper to get unique verses
    public List<string> GetVerses()
    {
        return allCharacters
            .Select(c => c.verse)
            .Distinct()
            .OrderBy(v => v)
            .ToList();
    }

    // Helper to get characters by verse
    public List<UnitDefinition> GetCharactersByVerse(string verse)
    {
        return allCharacters
            .Where(c => c.verse == verse)
            .ToList();
    }
}