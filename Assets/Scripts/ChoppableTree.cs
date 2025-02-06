using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour 
{
    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth;
    public float treeHealth;

    private Animator animator;

    private void Start()
    {
        treeHealth = treeMaxHealth;
        animator = transform.parent.transform.parent.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (canBeChopped)
        {
            GlobalState.instance.resourceHealth = treeHealth;
            GlobalState.instance.resourceMaxHealth = treeMaxHealth;
        }
    }

    public void GetHit()
    {
        StartCoroutine(Hit());
    }

    public void TreeIsDead()
    {
        Vector3 treePosition = transform.position;
        
        Destroy(transform.parent.transform.parent.gameObject);
        canBeChopped = false;
        SelectionManager.instance.selectedTree = null;
        SelectionManager.instance.chopHolder.gameObject.SetActive(false);
        SelectionManager.instance.axeCursor.SetActive(false);
        
        GameObject brokeTreen = Instantiate(Resources.Load<GameObject>("Prefabs/ChoppedTree"), treePosition, Quaternion.identity);
    }

    private IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.35f);
        animator.SetTrigger("Shake");
        treeHealth -= 1f;

        if (treeHealth <= 0)
        {
            TreeIsDead();
        }
        
    }
}
