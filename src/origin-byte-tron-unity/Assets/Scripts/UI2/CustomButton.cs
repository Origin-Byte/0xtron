using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;
    public TMP_Text text;
    
    public Color defaultTextColor;
    public Color hoveredTextColor;
    public Color pressedTextColor;

    protected override void Start()
    {
        base.Start();
        onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(clickSound);
        });
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        audioSource.PlayOneShot(hoverSound);
        text.color = hoveredTextColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        text.color = defaultTextColor;
    }
}
