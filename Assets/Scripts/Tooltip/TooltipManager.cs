using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;
    
    public TMP_Text tooltipName;
    public TMP_Text tooltipDescription;
    public Image tooltipIcon;
        
    public void ShowTooltip(string text, Sprite icon, string description)
    {
        tooltipName.text = text;
        tooltipDescription.text = description;
        tooltipIcon.sprite = icon;
        
        gameObject.SetActive(true);
    }
    
    public void HideTooltip()
    {
        tooltipName.text = string.Empty;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

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
}
