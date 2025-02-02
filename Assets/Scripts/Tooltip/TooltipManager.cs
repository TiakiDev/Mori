using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;
    
    public TMP_Text tooltipName;
    public Image tooltipIcon;
        
    public void ShowTooltip(string text, Sprite icon)
    {
        tooltipName.text = text;
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
