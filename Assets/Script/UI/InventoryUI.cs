using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : SelectionUI<ImageSlot>
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    private Inventory inventory;
    List<ItemSlotUI> slotUIList;
    private List<ItemSlot> currentSortedSlots;
    int selectedCategory = 0;

    void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;

        if (closeButton) closeButton.onClick.AddListener(OnBackButton);
    }

    void OnDestroy()
    {
        if (inventory != null) inventory.OnUpdated -= UpdateItemList;
        if (closeButton) closeButton.onClick.RemoveListener(OnBackButton);
    }

    void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        currentSortedSlots = inventory.GetSlotsByCategories(selectedCategory)
                            .OrderBy(slot => GetDetailedItemOrder(slot.Item))
                            .ThenBy(slot => slot.Item.Name)
                            .ToList();

        // Kumpulkan komponen yang selectable (ImageSlot) untuk SelectionUI
        var selectableItems = new List<ImageSlot>();

        for (int i = 0; i < currentSortedSlots.Count; i++)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(currentSortedSlots[i]);
            slotUIList.Add(slotUIObj);

            // Ambil komponen ImageSlot (bisa di object yg sama / child)
            var selectable = slotUIObj.GetComponent<ImageSlot>();
            if (selectable == null)
                selectable = slotUIObj.GetComponentInChildren<ImageSlot>(true);

            if (selectable == null)
            {
                Debug.LogError($"[InventoryUI] ImageSlot tidak ditemukan pada ItemSlotUI index {i}. Tambahkan komponen ImageSlot.");
                continue;
            }

            int capturedIndex = i;
            selectable.OnClick = () => OnItemClicked(capturedIndex);
            selectableItems.Add(selectable);
        }

        SetItems(selectableItems);

        UpdateSelectionInUI();
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }
    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();
        if (currentSortedSlots.Count > 0 && selectedItem >= 0 && selectedItem < currentSortedSlots.Count)
        {
            var item = currentSortedSlots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        else
        {
            itemIcon.sprite = null;
            itemDescription.text = "";
        }
    }
    int GetDetailedItemOrder(ItemBase item) => 1;
}

