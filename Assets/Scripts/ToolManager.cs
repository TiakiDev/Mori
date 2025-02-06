using System.Collections;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager instance;
    public Animator toolHolderAnimator;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void UseTool(Slot slot)
    {
        StartCoroutine(HittingRoutine(slot));
    }

    private IEnumerator HittingRoutine(Slot slot)
    {
        if (slot.itemSO.itemType == ItemSO.ItemType.Axe)
        {
            GameObject selectedTree = SelectionManager.instance.selectedTree;
        
            if (selectedTree != null)
            {
                selectedTree.GetComponent<ChoppableTree>().GetHit();
            }
        }

        if (slot.itemSO.itemType == ItemSO.ItemType.Pickaxe)
        {
            GameObject selectedOre = SelectionManager.instance.selectedOre;
        
            if (selectedOre != null)
            {
                selectedOre.GetComponent<MineableOre>().GetHit();
            }
        }
        GlobalState.instance.canUse = false;
        toolHolderAnimator.SetTrigger("Swing");
        yield return new WaitForSeconds(1f);
        GlobalState.instance.canUse = true;
    }
}