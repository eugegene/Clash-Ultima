using UnityEngine;

public enum Team
{
    Neutral,
    Red,
    Blue,
    Creeps
}

public static class TeamLogic
{
    public static bool IsEnemy(Team a, Team b)
    {
        if (a == Team.Neutral && b == Team.Neutral) return false;

        if (a == Team.Neutral || b == Team.Neutral) return true;

        return a != b;
    }
}