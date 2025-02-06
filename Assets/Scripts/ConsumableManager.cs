using System.Collections;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager instance;
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

    public void UseConsumable(Slot slot)
    {
        StartCoroutine(EatingRoutine(slot));
    }

    private IEnumerator EatingRoutine(Slot slot)
    {
        GlobalState.instance.canUse = false;
        toolHolderAnimator.SetTrigger("Eat");
        yield return new WaitForSeconds(0.35f);
        GlobalState.instance.canUse = true;
        
        StatsManager.instance.ChangeHunger(slot.itemSO.hungerAmount);
        StatsManager.instance.ChangeThirst(slot.itemSO.thirstAmount);
        StatsManager.instance.ChangeHealth(slot.itemSO.healthAmount);
        
        
        slot.RemoveItem(1);
    }
}