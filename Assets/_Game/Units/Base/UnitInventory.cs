using UnityEngine;
using UnityEngine.InputSystem;

public class UnitInventory : MonoBehaviour
{
    [Header("Config")]
    public int maxSlots = 2; // We wanted 2 slots
    
    // Runtime State
    public InventorySlot[] slots;
    private UnitStats _stats;

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        slots = new InventorySlot[maxSlots];
        for (int i = 0; i < maxSlots; i++) slots[i] = new InventorySlot();
    }

    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame) UseItem(0);
        if (Keyboard.current.jKey.wasPressedThisFrame) UseItem(1);
    }

    public void AddItem(ItemDefinition item, int amount)
    {
        // 1. Try to stack in existing slot
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.amount += amount;
                return;
            }
        }

        // 2. Try to find an empty slot
        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = amount;
                return;
            }
        }

        Debug.Log("Inventory Full!");
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        var slot = slots[slotIndex];
        if (slot.item != null && slot.amount > 0)
        {
            slot.item.OnUse(_stats);
            slot.amount--;
            
            if (slot.amount <= 0)
            {
                slot.item = null;
            }
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemDefinition item;
    public int amount;
}