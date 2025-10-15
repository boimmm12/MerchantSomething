using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { a, b, c }
public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> a;
    [SerializeField] List<ItemSlot> b;
    [SerializeField] List<ItemSlot> c;

    List<List<ItemSlot>> allSlots;
    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { a, b, c };
    }
    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "A", "B", "C"
    };
    public static Inventory GetInventory()
    {
        if (PlayerController.i == null)
        {
            Debug.LogError("[Inventory] PlayerController.i belum tersedia!");
            return null;
        }

        return PlayerController.i.GetComponent<Inventory>();
    }
    public List<ItemSlot> GetSlotsByCategories(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    ItemCategory GetCatagoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem || item is A)
            return ItemCategory.a;
        return ItemCategory.b;
    }
    public void AddItem(ItemBase item, int count = 1)
    {
        Debug.Log($"[Inventory] AddItem dipanggil: {item.name}, count: {count}");
        int category = (int)GetCatagoryFromItem(item);
        var currentSlots = GetSlotsByCategories(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        currentSlots.Sort((a, b) => string.Compare(a.Item.Name, b.Item.Name));

        OnUpdated?.Invoke();
    }

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCatagoryFromItem(item);
        var currentSlots = GetSlotsByCategories(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

        if (itemSlot != null)
            return itemSlot.Count;
        else
            return 0;
    }
    public void RemoveItem(ItemBase item, int countToRemove = 1)
    {
        int category = (int)GetCatagoryFromItem(item);
        var currentSlots = GetSlotsByCategories(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count -= countToRemove;
        if (itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

}
[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    public ItemSlot()
    {

    }
    public ItemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
}