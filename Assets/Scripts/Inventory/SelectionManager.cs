using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;
    
    public TMP_Text interactionText;
    //cursor variables
    public GameObject handCursor;
    public GameObject axeCursor;
    //targets variables
    public InteractableObject currentTarget;
    public bool onTarget;
    //tree variables
    public GameObject selectedTree;
    public GameObject chopHolder;

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
        Item item = selectionTransform.GetComponent<Item>();

        // Obsługa drzew
        if (choppableTree && choppableTree.playerInRange)
        {
            choppableTree.canBeChopped = true;
            selectedTree = choppableTree.gameObject;
            chopHolder.gameObject.SetActive(true);
            axeCursor.SetActive(true);
            anyCursorActive = true; // Aktywny kursor siekiery
        }
        else
        {
            if (selectedTree != null)
            {
                selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                selectedTree = null;
                chopHolder.gameObject.SetActive(false);
                axeCursor.SetActive(false);
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
        axeCursor.SetActive(false);
        handCursor.SetActive(false);
    }

    // Ustaw widoczność celownika na podstawie aktywnych kursorów
    FirstPersonController.instance.crosshairObject.gameObject.SetActive(!anyCursorActive);
}
}
