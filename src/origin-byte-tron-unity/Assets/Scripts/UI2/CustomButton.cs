using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CustomButton : Button
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;
    public TMP_Text text;
    
    public Color defaultTextColor;
    public Color hoveredTextColor;
    public Color pressedTextColor;

    public Sprite defaultImage;
    public Sprite hoveredImage;

    private Image _image;
    
    protected override void Start()
    {
        base.Start();
        onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(clickSound);
        });
        _image = GetComponent<Image>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _image.sprite = defaultImage;
        text.color = defaultTextColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        audioSource.PlayOneShot(hoverSound);
        text.color = hoveredTextColor;
        _image.sprite = hoveredImage;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        text.color = defaultTextColor;
        _image.sprite = defaultImage;
    }
}
