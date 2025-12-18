using UnityEngine;
using UnityEngine.UI;

public class AbilityHUD : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotContainer; 

    void Start()
    {
        // DEBUG MODE: Force icons to appear immediately
        GenerateTestSlots();
    }

    private void GenerateTestSlots()
    {
        Debug.Log("FORCING UI SPAWN (TEST MODE)");

        // Clear existing
        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        // Create 4 Dummy Slots
        for (int i = 0; i < 4; i++)
        {
            GameObject newSlot;

            if (slotPrefab != null)
            {
                newSlot = Instantiate(slotPrefab, slotContainer);
            }
            else
            {
                // If you forgot the prefab, create a raw image so you see SOMETHING
                newSlot = new GameObject("Dummy_Slot_" + i, typeof(Image));
                newSlot.transform.SetParent(slotContainer, false);
                newSlot.GetComponent<Image>().color = Color.green; // Green = No Prefab
            }
            
            // Force Size (in case Layout Group is crushing it)
            LayoutElement le = newSlot.AddComponent<LayoutElement>();
            le.preferredWidth = 100;
            le.preferredHeight = 100;
        }

        // Force Layout Refresh
        Canvas.ForceUpdateCanvases();
        if (slotContainer.GetComponent<ContentSizeFitter>()) 
            slotContainer.GetComponent<ContentSizeFitter>().enabled = false; // Disable fitter if it's bugging out
        if (slotContainer.GetComponent<HorizontalLayoutGroup>())
            slotContainer.GetComponent<HorizontalLayoutGroup>().enabled = false; // Toggle to refresh
        if (slotContainer.GetComponent<HorizontalLayoutGroup>())
            slotContainer.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}