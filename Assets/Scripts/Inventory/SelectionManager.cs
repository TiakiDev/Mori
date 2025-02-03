using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;
    
    public TMP_Text interactionText;
    public bool onTarget;

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
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
                
            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                
                interactionText.text = interactable.GetObjectName();
                interactionText.alpha = 1;
            }
            else
            {   onTarget = false;
                interactionText.alpha = 0;
            }
        }
        else
        {
            onTarget = false;
            interactionText.alpha = 0;
        }
    }
}
