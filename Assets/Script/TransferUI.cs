using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;

public class TransferUI : SelectionUI<ImageSlot>
{
    [SerializeField] Button RBoxM;
    [SerializeField] Button LBoxM;
    // [SerializeField] Button bButton;
    [SerializeField] Image movingPockiImage;
    [SerializeField] List<ImageSlot> boxSlots;

    bool isRbox = false;
    bool isLbox = false;

    List<BoxInventoryUI> inventSlots = new List<BoxInventoryUI>();
    List<BoxMarketUI> marketSlots = new List<BoxMarketUI>();
    Inventory inventory;
    MarketBox storageBoxes;
    List<Image> boxSlotImage = new List<Image>();
    private int selectedCategoryIndex = 0;
    public int SelectedBox { get; set; } = 0;
    private void Awake()
    {
        SetSelectionSettings(SelectionType.Grid, gridWidth: 6);
        inventSlots.Clear();
        marketSlots.Clear();

        foreach (var boxSlot in boxSlots)
        {
            var marketSlot = boxSlot.GetComponent<BoxMarketUI>();
            if (marketSlot != null)
            {
                marketSlots.Add(marketSlot);
            }
            else
            {
                var invSlot = boxSlot.GetComponent<BoxInventoryUI>();
                if (invSlot != null)
                    inventSlots.Add(invSlot);
            }
        }
        inventory = Inventory.GetInventory();
        storageBoxes = MarketBox.GetPlayerStorageBox();

        boxSlotImage = boxSlots.Select(b => b.transform.GetChild(0).GetComponent<Image>()).ToList();
        movingPockiImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetItems(boxSlots);
        var inv = Inventory.GetInventory();
        foreach (var slot in inv.GetSlotsByCategories(0))
        {
            Debug.Log($"Item: {slot.Item?.Name}, Count: {slot.Count}");
        }
        Debug.Log(storageBoxes != null
            ? $"[TransferUI] StorageBoxes OK, box count: {storageBoxes.NumOfBoxes}"
            : "[TransferUI] storageBoxes == NULL");

    }

    public void SetupActionButtons()
    {

        var items = GetComponentsInChildren<ImageSlot>().ToList();
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            items[i].OnClick = () =>
            {
                OnItemClicked(index);
                OnConfirmButton();
            };
        }

        // bButton.onClick.AddListener(() => OnBackButton());
        RBoxM.onClick.AddListener(() => isRbox = true);
        LBoxM.onClick.AddListener(() => isLbox = true);
    }

    public void SetDataInInventorySlots()
    {
        List<ItemSlot> src = null;
        try
        {
            src = inventory.GetSlotsByCategories(selectedCategoryIndex);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[TransferUI] GetSlotsByCategories({selectedCategoryIndex}) gagal: {e.Message}");
        }

        if (src == null) src = new List<ItemSlot>();

        for (int i = 0; i < inventSlots.Count; i++)
        {
            if (i < src.Count && src[i] != null && src[i].Item != null && src[i].Count > 0)
                inventSlots[i].SetData(src[i].Item, src[i].Count);
            else
                inventSlots[i].ClearData();
        }
    }

    public void SetDataInMarketSlots()
    {
        int boxIndex = SelectedBox;
        int totalSlots = storageBoxes.NumOfSlots;

        for (int i = 0; i < marketSlots.Count; i++)
        {
            if (i < totalSlots)
            {
                var item = storageBoxes.GetItem(boxIndex, i);
                if (item != null)
                    marketSlots[i].SetData(item);
                else
                    marketSlots[i].ClearData();
            }
            else
            {
                // kalau UI slot lebih banyak daripada jumlah slot sebenarnya, kosongkan sisanya
                marketSlots[i].ClearData();
            }
        }
    }

    public override void HandleUpdate()
    {
        int prevSelectedBox = SelectedBox;

        if (isLbox)
        {
            isLbox = false;
            SelectedBox = SelectedBox > 0 ? SelectedBox - 1 : storageBoxes.NumOfBoxes - 1;
        }
        else if (isRbox)
        {
            isRbox = false;
            SelectedBox = (SelectedBox + 1) % storageBoxes.NumOfBoxes;
        }

        if (prevSelectedBox != SelectedBox)
        {
            SetDataInMarketSlots();
            UpdateSelectionInUI();
            return;
        }

        base.HandleUpdate();
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        // boxNameText.text = "Box " + (SelectedBox + 1);

        if (movingPockiImage.gameObject.activeSelf)
            movingPockiImage.transform.position = boxSlotImage[selectedItem].transform.position + Vector3.up * 50f;
    }
}
