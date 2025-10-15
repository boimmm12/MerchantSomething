using System.Collections.Generic;
using UnityEngine;

public class MarketBox : MonoBehaviour
{
    const int numberOfBoxes = 2;
    const int numberOfSlot = 12;
    public int NumOfBoxes => numberOfBoxes;
    public int NumOfSlots => numberOfSlot;
    ItemBase[,] boxes = new ItemBase[numberOfBoxes, numberOfSlot];
    public void AddItem(ItemBase itemBase, int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = itemBase;
    }
    public void RemoveItem(int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = null;
    }

    public ItemBase GetItem(int boxIndex, int slotIndex)
    {
        return boxes[boxIndex, slotIndex];
    }

    public void AddItemToEmptySlot(ItemBase itemBase)
    {
        for (int boxIndex = 0; boxIndex < numberOfBoxes; boxIndex++)
        {
            for (int slotIndex = 0; slotIndex < numberOfSlot; slotIndex++)
            {
                if (boxes[boxIndex, slotIndex] == null)
                {
                    boxes[boxIndex, slotIndex] = itemBase;
                    return;
                }
            }
        }
    }
    public static MarketBox GetPlayerStorageBox()
    {
        return FindObjectOfType<PlayerController>().GetComponent<MarketBox>();
    }
}
