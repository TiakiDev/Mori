using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color color;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = Color.white;
    }
}
