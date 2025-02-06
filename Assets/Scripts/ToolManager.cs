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
        StartCoroutine(HittingRoutine());
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
}