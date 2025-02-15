using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;
    [Header("Refrences")]
    public TMP_Text interactionText;
    public GameObject infoHolder;
    //cursor variables
    [Header("Cursors")]
    public GameObject handCursor;
    public GameObject axeCursor;
    public GameObject pickaxeCursor;
    //targets variables
    [Space(2)]
    public InteractableObject currentTarget;
    public bool onTarget;
    //tree variables
    public GameObject selectedTree;
    public GameObject selectedOre;
    

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
        interactionText.alpha = 0;
    }
    
private void Update()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    currentTarget = null;
    bool anyCursorActive = false; // Flaga śledząca aktywność kursora

    if (Physics.Raycast(ray, out hit))
    {
        var selectionTransform = hit.transform;

        InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
        ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
        MineableOre mineableOre = selectionTransform.GetComponent<MineableOre>();
        Item item = selectionTransform.GetComponent<Item>();

        // Obsługa drzew
        if (choppableTree && choppableTree.playerInRange)
        {
            choppableTree.canBeChopped = true;
            selectedTree = choppableTree.gameObject;
            infoHolder.gameObject.SetActive(true);
            infoHolder.GetComponentInChildren<TMP_Text>().text = choppableTree.GetName();
            axeCursor.SetActive(true);
            anyCursorActive = true; // Aktywny kursor siekiery
        }
        else
        {
            if (selectedTree != null)
            {
                selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                selectedTree = null;
                infoHolder.gameObject.SetActive(false);
                axeCursor.SetActive(false);
            }
        }
        
        //obsługa mineralów
        if (mineableOre && mineableOre.playerInRange && !mineableOre.hasBeenMined)
        {
            mineableOre.canBeMined = true;
            selectedOre = mineableOre.gameObject;
            infoHolder.gameObject.SetActive(true);
            infoHolder.GetComponentInChildren<TMP_Text>().text = mineableOre.GetName();
            pickaxeCursor.SetActive(true);
            anyCursorActive = true; // Aktywny kursor kilofa
        }
        else
        {
            if (selectedOre != null) // This should be selectedOre
            {
                selectedOre.gameObject.GetComponent<MineableOre>().canBeMined = false;
                selectedOre = null;
                infoHolder.gameObject.SetActive(false);
                pickaxeCursor.SetActive(false);
            }
        }

        // Obsługa przedmiotów
        if (item && interactable.playerInRange && onTarget)
        {
            handCursor.SetActive(true);
            anyCursorActive = true; // Aktywny kursor ręki
        }
        else
        {
            handCursor.SetActive(false);
        }

        // Aktualizacja tekstu interakcji
        if (interactable && interactable.playerInRange)
        {
            onTarget = true;
            currentTarget = interactable;
            interactionText.text = interactable.GetObjectName();
            interactionText.alpha = 1;
        }
        else
        {
            onTarget = false;
            interactionText.alpha = 0;
        }
    }
    else
    {
        onTarget = false;
        interactionText.alpha = 0;
        pickaxeCursor.SetActive(false);
        axeCursor.SetActive(false);
        handCursor.SetActive(false);
        infoHolder.gameObject.SetActive(false); 
    }

    // Ustaw widoczność celownika na podstawie aktywnych kursorów
    FirstPersonController.instance.crosshairObject.gameObject.SetActive(!anyCursorActive);
}
}
