using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { a, b, c }
public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> a;
    [SerializeField] List<ItemSlot> b;
    [SerializeField] List<ItemSlot> c;

    List<List<ItemSlot>> allSlots;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { a, b, c };
    }
    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "A", "B", "C"
    };
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