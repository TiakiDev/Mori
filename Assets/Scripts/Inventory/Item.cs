using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemSO itemSO;
    public int quantity = 1;

    private InteractableObject interactable;

    private void Awake()
    {
        interactable = GetComponent<InteractableObject>();
    }
    

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && interactable.playerInRange && SelectionManager.instance.onTarget)
        {
            InventoryManager.instance.AddItem(itemSO, quantity);
            Destroy(gameObject);
        }
    }
}
