using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GhostItem : MonoBehaviour
{
    public BoxCollider solidCollider; // set manually
 
    public Renderer mRenderer;
    private Material fullTransparentMat;
    private Material selectedMaterial;
 
    public bool isPlaced;
 
    // A flag for the deletion algorithm
    public bool hasSamePosition = false;
    private void Start()
    {
        mRenderer = GetComponent<Renderer>();
        fullTransparentMat = ConstructionManager.instance.ghostFullTransparentMat;
        selectedMaterial = ConstructionManager.instance.ghostSelectedMat;
 
        mRenderer.material = fullTransparentMat;

        solidCollider.enabled = false;
    }
 
    private void Update()
    {
        if (ConstructionManager.instance.inConstructionMode)
        {
            Collider playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
            foreach (Collider col in GetComponents<Collider>())
            {
                Physics.IgnoreCollision(col, playerCollider);
            }

        }
        
        // We need the solid collider so the ray cast will detect it
        if (ConstructionManager.instance.inConstructionMode && isPlaced)
        {
            solidCollider.enabled = true;
        }
 
        if (!ConstructionManager.instance.inConstructionMode)
        {
            solidCollider.enabled = false;
        }
 
        // Triggering the material
        if(ConstructionManager.instance.selectedGhost == this.gameObject)
        {
            mRenderer.material = selectedMaterial;
        }
        else
        {
            mRenderer.material = fullTransparentMat; //change to semi if in debug else full
        }
    }
}