using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;
    public GameObject textPrefab; // Drag your prefab here
    public Canvas worldCanvas;    // Drag your WorldSpace Canvas here

    void Awake() { Instance = this; }

    public void ShowDamage(float amount, Vector3 position)
    {
        if (textPrefab == null || worldCanvas == null) return;

        // Spawn inside the canvas
        GameObject textObj = Instantiate(textPrefab, worldCanvas.transform);
        
        // Position it slightly above the unit
        textObj.transform.position = position + Vector3.up * 2f;
        
        // Setup
        textObj.GetComponent<FloatingText>().Setup(amount);
        
        // Make it face the camera
        textObj.transform.rotation = Camera.main.transform.rotation;
    }
}