using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Text countText;

    RectTransform rectTransform;
    private void Awake()
    {
        
    }

    public Image ItemImage => itemImage;
    public Text CountText => countText;

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        itemImage.sprite = itemSlot.Item.Icon;
        countText.text = $"X {itemSlot.Count}";
    }

    public void SetNameAndPrice(ItemBase item)
    {
        rectTransform = GetComponent<RectTransform>();
        itemImage.sprite = item.Icon;
        countText.text = $"©{item.Price}";
    }
}
