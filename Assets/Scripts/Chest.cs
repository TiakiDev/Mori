using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject
{
    
    public List<ChestSlot> slots = new List<ChestSlot>();
    
    public void Interact()
    {
        InventoryManager.instance.OpenChest(this);
    }
}

[System.Serializable]
public class ChestSlot
{
    public ItemSO itemSO;
    public int quantity;
}
