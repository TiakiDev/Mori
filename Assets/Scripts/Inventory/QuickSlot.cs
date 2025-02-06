using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class QuickSlot : MonoBehaviour
{
    private Slot slot;
    public GameObject selectedShader;
    public bool isSelected;
    public GameObject toolHolder;
    public GameObject itemModel;
    
    private bool isEquipped;
    private Animator toolHolderAnimator;
    
    private void Start()
    {
        slot = GetComponent<Slot>();
        selectedShader.SetActive(false);
        toolHolderAnimator = toolHolder.GetComponent<Animator>();

        isEquipped = false;
    }
    
    private void Update()
    {
        if (isSelected && (slot.itemSO == null || slot.quantity <= 0))
        {
            UnequipItem();
            isEquipped = false;
        }
        
        if(isSelected && slot.itemSO != null && !isEquipped)
        {
            SetEquippedItem(slot.itemSO);
            isEquipped = true;
        }
        
        if(Input.GetKeyDown(KeyCode.Mouse0) && isSelected && slot.itemSO != null 
           && !InventoryManager.instance.isOpen && !SelectionManager.instance.onTarget && GlobalState.instance.canUse)
        {
            if (slot.itemSO.itemType == ItemSO.ItemType.Consumable)
            {
                StartCoroutine(EatingRoutine());
            }
            else if (slot.itemSO.itemType == ItemSO.ItemType.Axe)
            {
                StartCoroutine(HittingRoutine());
            }
        }
    }
    
    private IEnumerator EatingRoutine()
    {
        GlobalState.instance.canUse = false;
        toolHolderAnimator.SetTrigger("Eat");
        yield return new WaitForSeconds(0.35f);
        GlobalState.instance.canUse = true;
        slot.quantity -= 1;
        slot.UpdateQuantityText();
        if (slot.quantity <= 0)
        {
            slot.ClearSlot();
                    
        }
    }
    
    private IEnumerator HittingRoutine()
    {
        GameObject selectedTree = SelectionManager.instance.selectedTree;

        if (selectedTree != null)
        {
            selectedTree.GetComponent<ChoppableTree>().GetHit();
        }
        GlobalState.instance.canUse = false;
        toolHolderAnimator.SetTrigger("Swing");
        yield return new WaitForSeconds(1f);
        GlobalState.instance.canUse = true;
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
        GameObject modelPrefab = Resources.Load<GameObject>("EquipableModels/" + item.itemName + "_Model");

        if (modelPrefab != null)
        {
            itemModel = Instantiate(
                modelPrefab,
                toolHolder.transform.position, // pozycja
                Quaternion.identity, // rotacja
                toolHolder.transform // rodzic
            );
            
            itemModel.transform.localPosition = new Vector3(0.070f, 0.240f, 0.010f);
            itemModel.transform.localRotation = Quaternion.Euler(-40f, -105, -8f);
            itemModel.gameObject.name = item.itemName;
            
            itemModel.layer = LayerMask.NameToLayer("Tool");
            var modelRenderer = itemModel.GetComponent<MeshRenderer>();
            
            if (modelRenderer == null)
            {
                modelRenderer = itemModel.GetComponentInChildren<MeshRenderer>();
            }
            if (modelRenderer != null)
            {
                modelRenderer.material.shader = Shader.Find("Custom/OverlayLitShader");
                modelRenderer.shadowCastingMode = ShadowCastingMode.Off;
            }

            isEquipped = true;
        }
        else
        {
            Debug.LogError("Model nie został znaleziony: " + item.itemName);
        }
    }
}
