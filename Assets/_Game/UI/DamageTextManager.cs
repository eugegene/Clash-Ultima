using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;
    public GameObject textPrefab; // Drag your prefab here
    public Canvas worldCanvas;    // Drag your WorldSpace Canvas here

    void Awake() { Instance = this; }

    public void ShowDamage(float amount, Vector3 position, bool isCrit = false) 
    {
        if (textPrefab == null || worldCanvas == null) return;

        GameObject textObj = Instantiate(textPrefab, worldCanvas.transform);
        textObj.transform.position = position + Vector3.up * 2f;
        
        // Get the script
        FloatingText ft = textObj.GetComponent<FloatingText>();
        ft.Setup(amount);

        // --- NEW VISUAL LOGIC ---
        if (isCrit)
        {
            ft.GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow; // Make crits Yellow!
            textObj.transform.localScale *= 1.5f; // Make them BIG!
        }
        // ------------------------

        textObj.transform.rotation = Camera.main.transform.rotation;
    }
}