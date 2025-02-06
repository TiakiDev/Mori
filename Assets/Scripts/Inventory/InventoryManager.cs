using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public bool isOpen;

    [SerializeField] private GameObject inventoryPanel;
    
    private int currentSlotIndex = 0;  // Aktualnie wybrany slot
    private float accumulatedScroll;   // Akumulowana wartość scrolla
    
    public List<Slot> itemSlots = new List<Slot>();
    public List<QuickSlot> quickSlots = new List<QuickSlot>();

    public void AddItem(ItemSO item, int amount = 1)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemSO == item)
            {
                itemSlots[i].AddItem(item, amount);
                return;
            }
        }
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemSO == null)
            {
                itemSlots[i].AddItem(item, amount);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < quickSlots.Count; i++)
        {
            quickSlots[i].isSelected = false;
            quickSlots[i].selectedShader.SetActive(false);
            quickSlots[i].UnequipItem();
        }
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        inventoryPanel.SetActive(false);
    }
    
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab) && isOpen)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
            
            FirstPersonController.instance.lockCursor = true;
            FirstPersonController.instance.cameraCanMove = true;
            FirstPersonController.instance.crosshairObject.gameObject.SetActive(true);
            SelectionManager.instance.interactionText.gameObject.SetActive(true);
            TooltipManager.instance.HideTooltip();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && !isOpen)
        {
            inventoryPanel.SetActive(true);
            isOpen = true;
            FirstPersonController.instance.lockCursor = false;
            FirstPersonController.instance.cameraCanMove = false;
            FirstPersonController.instance.crosshairObject.gameObject.SetActive(false);
            SelectionManager.instance.interactionText.gameObject.SetActive(false);
        }
        
        SlotChangingHandler();
        
    }
    
    private void SlotChangingHandler()
    {
        //możesz se zmieniac sloty 1 2 3 4
        // Obsługa klawiszy 1-6
        for (int i = 0; i < quickSlots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentSlotIndex = i;
                quickSlots[i].SelectSlot();
                break;
            }
        }

        // Obsługa scrolla
        accumulatedScroll += Input.GetAxis("Mouse ScrollWheel");
        
        // Sprawdź czy scroll wystarczająco się przemieścił (0.1 = 1 "kliknięcie" scrolla)
        if (Mathf.Abs(accumulatedScroll) >= 0.1f)
        {
            int steps = (int)(accumulatedScroll * 10); // Każdy "klik" to 0.1, mnożymy przez 10 by dostać liczbę kroków
            steps *= -1;
            currentSlotIndex += steps;

            // Zawijanie indeksu (np. od 0 do 5 dla 6 slotów)
            currentSlotIndex = (currentSlotIndex % quickSlots.Count + quickSlots.Count) % quickSlots.Count;

            quickSlots[currentSlotIndex].SelectSlot();
            accumulatedScroll = 0f; // Reset akumulacji
        }
    }
    
}
