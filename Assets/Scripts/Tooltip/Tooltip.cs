using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Slot slot;

    private string tooltipName;
    private Sprite tooltipIcon;
    
    private void Start()
    {
        slot = GetComponent<Slot>();
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot.itemSO != null)
        {
            tooltipName = slot.itemSO.itemName;
            tooltipIcon = slot.itemSO.itemIcon;
            TooltipManager.instance.ShowTooltip(tooltipName, tooltipIcon);
        }
        else
        {
            TooltipManager.instance.HideTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}
