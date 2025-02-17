using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager instance;
 
    public GameObject itemToBeConstructed;
    public bool inConstructionMode = false;
    public GameObject constructionHoldingSpot;
 
    public bool isValidPlacement;
 
    public bool selectingAGhost;
    public GameObject selectedGhost;
 
    // Materials we store as refereces for the ghosts
    public Material ghostSelectedMat;
    public Material ghostFullTransparentMat;
 
    // We keep a reference to all ghosts currently in our world,
    // so the manager can monitor them for various operations
    public List<GameObject> allGhostsInExistence = new List<GameObject>();
 
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
 
    public void ActivateConstructionPlacement(string itemToConstruct)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>(itemToConstruct));
 
        //change the name of the gameobject so it will not be (clone)
        item.name = itemToConstruct;
 
        item.transform.SetParent(constructionHoldingSpot.transform, false);
        itemToBeConstructed = item;
        itemToBeConstructed.gameObject.tag = "activeConstructable";
 
        // Disabling the non-trigger collider so our mouse can cast a ray
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = false;
 
        // Actiavting Construction mode
        inConstructionMode = true;
    }
 
    private void GetAllGhosts(GameObject itemToBeConstructed)
    {
        List<GameObject> ghostlist = itemToBeConstructed.gameObject.GetComponent<Constructable>().ghostList;
 
        foreach (GameObject ghost in ghostlist)
        {
            Debug.Log(ghost);
            allGhostsInExistence.Add(ghost);
        }
    }
 
    private void PerformGhostDeletionScan()
    {
 
        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition == false) // if we did not already add a flag
                {
                    foreach (GameObject ghostX in allGhostsInExistence)
                    {
                        // First we check that it is not the same object
                        if (ghost.gameObject != ghostX.gameObject)
                        {
                            // If its not the same object but they have the same position
                            if (XPositionToAccurateFloat(ghost) == XPositionToAccurateFloat(ghostX) && ZPositionToAccurateFloat(ghost) == ZPositionToAccurateFloat(ghostX))
                            {
                                if (ghost != null && ghostX != null)
                                {
                                    // setting the flag
                                    ghostX.GetComponent<GhostItem>().hasSamePosition = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
 
        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition)
                {
                    DestroyImmediate(ghost);
                }
            }
 
        }
    }
 
    private float XPositionToAccurateFloat(GameObject ghost)
    {
        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost.gameObject.transform.position;
            float pos = targetPosition.x;
            float xFloat = Mathf.Round(pos * 100f) / 100f;
            return xFloat;
        }
        return 0;
    }
 
    private float ZPositionToAccurateFloat(GameObject ghost)
    {
 
        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost.gameObject.transform.position;
            float pos = targetPosition.z;
            float zFloat = Mathf.Round(pos * 100f) / 100f;
            return zFloat;
 
        }
        return 0;
    }
 
private void Update()
{
    if (inConstructionMode)
    {
        InventoryManager.instance.canChangeSlots = false;
    }
    else
    {
        InventoryManager.instance.canChangeSlots = true;
    }
    

    if (itemToBeConstructed != null && inConstructionMode)
    {
        //metoda obracania w górę i w dół za pomocą scrolla 
        if (!selectingAGhost && itemToBeConstructed.name == "FoundationModel")
        {
            float rotationSpeed = 90f; // Stopni na kliknięcie
            float rotationAmount = 45f; // Maksymalny kąt obrotu w górę/w dół

            // Obracanie w górę i w dół za pomocą scrolla (wokół osi X)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float currentRotationX = itemToBeConstructed.transform.eulerAngles.x;
                float newRotationX = currentRotationX + scroll * rotationAmount;

                // Ograniczenie obrotu do 45 stopni w górę i -45 stopni w dół
                if (newRotationX > 315f) newRotationX -= 360f; // Normalizacja kąta
                newRotationX = Mathf.Clamp(newRotationX, -25f, 15f);

                itemToBeConstructed.transform.eulerAngles = new Vector3(newRotationX, itemToBeConstructed.transform.eulerAngles.y, itemToBeConstructed.transform.eulerAngles.z);
            }
        }

        // Sprawdzenie poprawności umiejscowienia budowli
        if (itemToBeConstructed.name == "FoundationModel")
        {
            if (CheckValidConstructionPosition())
            {
                isValidPlacement = true;
                itemToBeConstructed.GetComponent<Constructable>().SetValidColor();
            }
            else
            {
                isValidPlacement = false;
                itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
            }
        }
        else
        {
            itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var selectionTransform = hit.transform;
            if (selectionTransform.gameObject.CompareTag("ghost") && itemToBeConstructed.name == "FoundationModel")
            {
                itemToBeConstructed.SetActive(false);
                selectingAGhost = true;
                selectedGhost = selectionTransform.gameObject;
            }
            else if (selectionTransform.gameObject.CompareTag("wallGhost") && itemToBeConstructed.name == "WallModel")
            {
                itemToBeConstructed.SetActive(false);
                selectingAGhost = true;
                selectedGhost = selectionTransform.gameObject;
            }
            else
            {
                itemToBeConstructed.SetActive(true);
                selectedGhost = null;
                selectingAGhost = false;
            }
        }
    }

    // Kliknięcie lewym przyciskiem myszy, aby umieścić obiekt
    if (Input.GetMouseButtonDown(0) && inConstructionMode && itemToBeConstructed != null)
    {
        if (isValidPlacement && selectedGhost == null && itemToBeConstructed.name == "FoundationModel")
        {
            PlaceItemFreeStyle();
        }

        if (selectingAGhost)
        {
            PlaceItemInGhostPosition(selectedGhost);
        }
    }

    // Kliknięcie prawym przyciskiem myszy, aby wyjść z trybu budowy
    if (Input.GetMouseButtonDown(1))
    {
        ExitConstructionMode();
    }
}


    public void ExitConstructionMode()
    {
        if (itemToBeConstructed != null)
        {
            Destroy(itemToBeConstructed);
            itemToBeConstructed = null;
        }
        inConstructionMode = false;
        selectedGhost = null;
        selectingAGhost = false;
    }
 
    private void PlaceItemInGhostPosition(GameObject copyOfGhost)
    {
 
        var randomOffset = UnityEngine.Random.Range(0.01f, 0.03f);
        Vector3 ghostPosition = copyOfGhost.transform.position;
        Quaternion ghostRotation = copyOfGhost.transform.rotation;
 
        selectedGhost.gameObject.SetActive(false);
 
        // Setting the item to be active again (after we disabled it in the ray cast)
        itemToBeConstructed.gameObject.SetActive(true);
        // Setting the parent to be the root of our scene
        itemToBeConstructed.transform.SetParent(transform.parent, true);

        itemToBeConstructed.transform.position = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z  + randomOffset);

        itemToBeConstructed.transform.rotation = ghostRotation;
        
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
 
        // Enabling back the solider collider that we disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;
 
        //Adding all the ghosts of this item into the manager's ghost bank
        if (itemToBeConstructed.name == "FoundationModel")
        {
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
            itemToBeConstructed.tag = "placedFoundation";
            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();
        }
        else
        {
            itemToBeConstructed.tag = "placedWall";
            Destroy(selectedGhost);
        }
 
        itemToBeConstructed = null;
 
        inConstructionMode = false;
    }
 
 
    private void PlaceItemFreeStyle()
    {
        // Setting the parent to be the root of our scene
        itemToBeConstructed.transform.SetParent(transform.parent, true);
 
        // Making the Ghost Children to no longer be children of this item
        itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
        // Setting the default color/material
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
        itemToBeConstructed.tag = "placedFoundation";
        itemToBeConstructed.GetComponent<Constructable>().enabled = false;
        // Enabling back the solider collider that we disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;
 
        //Adding all the ghosts of this item into the manager's ghost bank
        
        GetAllGhosts(itemToBeConstructed);
        PerformGhostDeletionScan();
 
        itemToBeConstructed = null;
 
        inConstructionMode = false;
        
    }
 
    private bool CheckValidConstructionPosition()
    {
        if (itemToBeConstructed != null)
        {
            return itemToBeConstructed.GetComponent<Constructable>().isValidToBeBuilt;
        }
 
        return false;
    }
}