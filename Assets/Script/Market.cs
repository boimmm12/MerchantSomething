using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class Market : MonoBehaviour, Interactable
{
    [SerializeField] List<ItemSlot> availableSlots = new List<ItemSlot>();
    [SerializeField] Transform entrancePoint;
    bool sellFinished;
    Customer waitingCustomer;
    ItemBase waitingItem;
    int waitingSlotIndex = -1;
    public List<ItemSlot> GetAvailableItemsWithCounts()
        => availableSlots.Where(s => s != null && s.Item != null && s.Count > 0)
                        .Select(s => new ItemSlot { Item = s.Item, Count = s.Count })
                        .ToList();

    public Vector3 GetEntrancePosition()
        => entrancePoint ? entrancePoint.position : transform.position;
    public bool TryReserveRandomAvailableItem(out ItemBase item)
    {
        var candidates = Enumerable.Range(0, availableSlots.Count)
            .Where(i => availableSlots[i] != null && availableSlots[i].Item != null && availableSlots[i].Count > 0)
            .ToList();

        if (candidates.Count == 0)
        {
            item = null;
            return false;
        }

        waitingSlotIndex = candidates[Random.Range(0, candidates.Count)];
        item = availableSlots[waitingSlotIndex].Item;
        return true;
    }
    public bool TrySetWaiting(Customer cust, ItemBase item)
    {
        if (waitingCustomer != null && waitingCustomer != cust) return false;
        waitingCustomer = cust;
        waitingItem = item;
        sellFinished = false;
        return true;
    }
    public void ClearWaiting(Customer cust)
    {
        if (waitingCustomer == cust)
        {
            waitingCustomer = null;
            waitingItem = null;
            sellFinished = false;
            waitingSlotIndex = -1;
        }
    }
    public void CommitSale(Customer cust)
    {
        if (waitingCustomer != cust || waitingSlotIndex < 0) { sellFinished = true; return; }

        var slot = availableSlots[waitingSlotIndex];
        slot.Count = Mathf.Max(0, slot.Count - 1);
        sellFinished = true;
    }
    public void CounterSale(Customer cust)
    {
        if (waitingCustomer != cust || waitingSlotIndex < 0) { sellFinished = true; return; }

        var slot = availableSlots[waitingSlotIndex];
        slot.Count = Mathf.Max(0, slot.Count - 1);
        sellFinished = true;
    }
    public void CancelSale(Customer cust)
    {
        if (waitingCustomer != cust) return;
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
        if (waitingCustomer != null && waitingItem != null)
        {
            yield return GameController.Instance.StateMachine.PushAndWait(SellingState.i);
            yield break;
        }

        Debug.Log($"{name}: Tidak ada customer yang menunggu.");
        yield return null;
    }
    public ItemBase SelectedItem => waitingItem;
    public Customer SelectedCustomer => waitingCustomer;
    public int GetPriceFor(ItemBase item) => item ? item.Price : 0;
    public void MarkSellFinished() => sellFinished = true;
}
