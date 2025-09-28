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
        if (item is RecoveryItem)
            return ItemCategory.a;
        return ItemCategory.b;
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