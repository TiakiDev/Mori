using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    private Slot slot;
    public GameObject selectedShader;
    public bool isSelected;
    public GameObject toolHolder;
    public GameObject itemModel;
    
    private void Start()
    {
        slot = GetComponent<Slot>();
        selectedShader.SetActive(false);
    }
    
    private void Update()
    {
        // Jeśli slot jest wybrany, ale nie ma w nim przedmiotu - zdejmij go
        if (isSelected && (slot.itemSO == null || slot.quantity <= 0))
        {
            UnequipItem();
        }
        
        if(Input.GetKeyDown(KeyCode.Mouse0) && isSelected && slot.itemSO != null)
        {
            if (slot.itemSO.itemType == ItemSO.ItemType.Consumable)
            {
                slot.quantity -= 1;
                slot.UpdateQuantityText();
                if (slot.quantity <= 0)
                {
                    slot.ClearSlot();
                    
                }
            }
        }
    }

    public void SelectSlot()
    {
        InventoryManager.instance.DeselectAllSlots();
        selectedShader.SetActive(true);
        isSelected = true;
        if (slot.itemSO != null)
        {
            SetEquippedItem(slot.itemSO);
        }
    }
    
    public void UnequipItem()
    {
        if (itemModel != null)
        {
            Destroy(itemModel);
            itemModel = null;
        }
    }
    
    private void SetEquippedItem(ItemSO item)
    {
        // Ładujesz model z folderu Resources/Art/Models
        GameObject modelPrefab = Resources.Load<GameObject>("Art/Models/" + item.itemName + "_Model");

        if (modelPrefab != null)
        {
            // Instancjonuj model, przypisz do toolHolder
            itemModel = Instantiate(
                modelPrefab,
                toolHolder.transform.position,
                Quaternion.identity,
                toolHolder.transform // rodzic
            );
            
            itemModel.transform.localPosition = new Vector3(0.070f, 0.240f, 0.010f);
            itemModel.transform.localRotation = Quaternion.Euler(-40f, -105, -8f);
            itemModel.gameObject.name = item.itemName;
        }
        else
        {
            Debug.LogError("Model nie został znaleziony: " + item.itemName);
        }
    }
    public void UseItem()
    {
            
    }
}
