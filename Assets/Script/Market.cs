using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Market : MonoBehaviour, Interactable
{
    [SerializeField] Transform entrancePoint;

    MarketBox storageBoxes;

    // waiting state (pilihan pelanggan)
    bool sellFinished;
    Customer waitingCustomer;
    ItemBase waitingItem;
    int waitingBoxIndex = -1;
    int waitingSlotIndex = -1;

    public bool isOpen = false;

    public event System.Action OnUpdated; // optional: UI bisa subscribe

    void Awake()
    {
        storageBoxes = MarketBox.GetPlayerStorageBox();
    }

    public List<ItemSlot> GetAvailableItemsWithCounts()
    {
        var result = new List<ItemSlot>();
        if (storageBoxes == null) return result;

        for (int b = 0; b < storageBoxes.NumOfBoxes; b++)
        {
            for (int s = 0; s < storageBoxes.NumOfSlots; s++)
            {
                var item = storageBoxes.GetItem(b, s);
                if (item != null)
                {
                    result.Add(new ItemSlot { Item = item, Count = 1 });
                }
            }
        }

        return result;
    }

    public Vector3 GetEntrancePosition()
        => entrancePoint ? entrancePoint.position : transform.position;

    public bool TryReserveRandomAvailableItem(out ItemBase item)
    {
        item = null;
        if (storageBoxes == null) return false;

        var candidates = new List<(int boxIndex, int slotIndex)>();

        for (int b = 0; b < storageBoxes.NumOfBoxes; b++)
        {
            for (int s = 0; s < storageBoxes.NumOfSlots; s++)
            {
                var it = storageBoxes.GetItem(b, s);
                if (it != null) candidates.Add((b, s));
            }
        }

        if (candidates.Count == 0) return false;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        waitingBoxIndex = chosen.boxIndex;
        waitingSlotIndex = chosen.slotIndex;
        waitingItem = storageBoxes.GetItem(waitingBoxIndex, waitingSlotIndex);
        item = waitingItem;
        return true;
    }

    public bool TrySetWaiting(Customer cust, ItemBase item)
    {
        if (storageBoxes == null) return false;
        if (waitingCustomer != null && waitingCustomer != cust) return false;

        // cari lokasi item di storageBoxes (ambil first match)
        for (int b = 0; b < storageBoxes.NumOfBoxes; b++)
        {
            for (int s = 0; s < storageBoxes.NumOfSlots; s++)
            {
                var it = storageBoxes.GetItem(b, s);
                if (it == item)
                {
                    waitingCustomer = cust;
                    waitingItem = item;
                    waitingBoxIndex = b;
                    waitingSlotIndex = s;
                    sellFinished = false;
                    return true;
                }
            }
        }

        return false;
    }

    public void ClearWaiting(Customer cust)
    {
        if (waitingCustomer == cust)
        {
            waitingCustomer = null;
            waitingItem = null;
            sellFinished = false;
            waitingBoxIndex = -1;
            waitingSlotIndex = -1;
        }
    }

    public void CommitSale(Customer cust)
    {
        if (waitingCustomer == null || waitingCustomer != cust)
        {
            sellFinished = true;
            return;
        }

        if (waitingBoxIndex < 0 || waitingSlotIndex < 0)
        {
            sellFinished = true;
            return;
        }

        storageBoxes.RemoveItem(waitingBoxIndex, waitingSlotIndex);

        waitingCustomer = null;
        waitingItem = null;
        waitingBoxIndex = -1;
        waitingSlotIndex = -1;
        sellFinished = true;

        OnUpdated?.Invoke();
    }

    public void CounterSale(Customer cust)
    {
        CommitSale(cust);
    }

    public void CancelSale(Customer cust)
    {
        if (waitingCustomer != cust) return;
        waitingCustomer = null;
        waitingItem = null;
        waitingBoxIndex = -1;
        waitingSlotIndex = -1;
        sellFinished = true;
    }

    public bool ConsumeSellFinished(Customer cust)
    {
        if (waitingCustomer != cust) return false;
        if (!sellFinished) return false;
        sellFinished = false;
        return true;
    }

    public IEnumerator Interact(Transform initiator)
    {
        storageBoxes = MarketBox.GetPlayerStorageBox();
        if (isOpen)
        {
            if (waitingCustomer != null && waitingItem != null)
            {
                yield return GameController.Instance.StateMachine.PushAndWait(SellingState.i);
                yield break;
            }
        }
        else
        {
            yield return GameController.Instance.StateMachine.PushAndWait(TransferState.i);
            yield break;
        }
    }

    public ItemBase SelectedItem => waitingItem;
    public Customer SelectedCustomer => waitingCustomer;
    public int GetPriceFor(ItemBase item) => item ? item.Price : 0;
    public void MarkSellFinished() => sellFinished = true;
}
