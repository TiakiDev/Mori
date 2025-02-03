using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject dragIcon;

    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite nullImage;
    
    public ItemSO itemSO;
    public int quantity;
    

    public void AddItem(ItemSO item, int amount)
    {
        itemSO = item;
        quantity += amount;

        quantityText.text = quantity.ToString();
        icon.sprite = itemSO.itemIcon;
        
        UpdateQuantityText();
    }

    private void Start()
    {
        UpdateQuantityText();
    }

    private void ClearSlot()
    {
        icon.sprite = nullImage;
        itemSO = null;
        quantity = 0;
        UpdateQuantityText();
    }
    
    private void UpdateQuantityText()
    {
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    { 
        if(itemSO == null) return;
        
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root);
        dragIcon.transform.SetAsLastSibling();
        Image dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = icon.sprite;
        dragImage.raycastTarget = false;
        
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            icon.sprite = nullImage;
            quantityText.enabled = false; 
        }
        else if (eventData.button == PointerEventData.InputButton.Right && quantity == 1)
        {
            icon.sprite = nullImage;
            quantityText.enabled = false; 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        quantityText.enabled = true;
        if (itemSO == null) return;
        icon.sprite = itemSO.itemIcon;
        TooltipManager.instance.ShowTooltip(itemSO.itemName, itemSO.itemIcon);

        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }
        
        if (eventData.pointerEnter != null)
        {
            Transform current = eventData.pointerEnter.transform;
            while (current != null)
            {
                if (current.TryGetComponent<Slot>(out Slot itemSlot))
                {
                    if (itemSlot == this)
                    {
                        return;
                    }   
                    
                    if (itemSlot.itemSO == null)
                    {
                        if (eventData.button == PointerEventData.InputButton.Left)
                        {
                            itemSlot.AddItem(itemSO, quantity);
                            ClearSlot();
                        }
                        if (eventData.button == PointerEventData.InputButton.Right)
                        {
                            if (quantity <= 1)
                            {
                                itemSlot.AddItem(itemSO, quantity);
                                ClearSlot();
                            }
                            else
                            {
                                itemSlot.AddItem(itemSO, quantity / 2);
                                quantity -= quantity / 2;
                            }
                            UpdateQuantityText();
                            return;
                        }

                    }
                    if(itemSlot.itemSO != null && itemSlot.itemSO == itemSO)
                    {
                        itemSlot.AddItem(itemSO, quantity);
                        ClearSlot();
                        UpdateQuantityText();
                        return;
                    }
                    if (itemSlot.itemSO != null && itemSlot.itemSO != itemSO)
                    {
                        SwapItems(itemSlot);
                        UpdateQuantityText();
                        return;
                    }
                    
                }
                current = current.parent;
            }
        }
    }
    private void SwapItems(Slot otherSlot)
    {
        if (itemSO == null || otherSlot.itemSO == null) return; 
        
        ItemSO tempItem = itemSO;
        int tempQuantity = quantity;

        ClearSlot();  
        AddItem(otherSlot.itemSO, otherSlot.quantity);  

        otherSlot.ClearSlot();  
        otherSlot.AddItem(tempItem, tempQuantity); 

        UpdateQuantityText();
    }
}
