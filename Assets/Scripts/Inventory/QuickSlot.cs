using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    private Slot slot;
    public GameObject selectedShader;
    public bool isSelected;
    
    private void Start()
    {
        slot = GetComponent<Slot>();
        selectedShader.SetActive(false);
    }

    public void SelectSlot()
    {
        InventoryManager.instance.DeselectAllSlots();
        selectedShader.SetActive(true);
        isSelected = true;
    }
    
    
}
