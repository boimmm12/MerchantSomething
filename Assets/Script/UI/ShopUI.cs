using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class ShopUI : SelectionUI<TextSlot>
{
    [SerializeField] Text nameText;
    [SerializeField] Text priceText;
    [SerializeField] Text negoText;
    [SerializeField] Image itemImage;
    [SerializeField] Sprite placeholderSprite;
    void Start()
    {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
        SetupButtons();
    }
    public void ShowNamePrice(string itemName, int price, Sprite sprite, int nego)
    {
        if (nameText) nameText.text = string.IsNullOrEmpty(itemName) ? "-" : itemName;
        if (priceText) priceText.text = price.ToString();
        if (itemImage)
        {
            itemImage.sprite = sprite ? sprite : placeholderSprite;
            itemImage.enabled = itemImage.sprite != null;
            itemImage.preserveAspect = true;
        }
        if(negoText) negoText.text = nego.ToString();

        gameObject.SetActive(true);
    }
    public void SetupButtons()
    {
        var items = GetComponentsInChildren<TextSlot>().ToList();
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            items[i].OnClick = () =>
            {
                OnItemClicked(index);
                OnConfirmButton();
            };
        }
    }
    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }

    public void Hide() => gameObject.SetActive(false);
}
