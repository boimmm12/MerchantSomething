using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Button closeButton;
    private Inventory inventory;
    List<ItemSlotUI> slotUIList;
    private List<ItemSlot> currentSortedSlots;
    int selectedCategory = 0;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    private void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
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

        foreach (var itemSlot in currentSortedSlots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }
    }
    void Update()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnBackButton);
        if (Input.GetKeyDown(KeyCode.Tab))
            OnBackButton();
    }
    void OnBackButton()
    {
        gameObject.SetActive(false);
    }
    int GetDetailedItemOrder(ItemBase item)
    {
        return 1;
    }
}
