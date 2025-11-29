using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    public UnitDefinition SelectedCharacter; // The chosen hero

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
