using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Market yang menggunakan data langsung dari MarketBox (storage pemain).
/// Tidak menyimpan salinan item — semua operasi (reserve/commit) mengubah MarketBox.
/// </summary>
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

    public event System.Action OnUpdated; // optional: UI bisa subscribe

    void Awake()
    {
        storageBoxes = MarketBox.GetPlayerStorageBox();
    }

    void OnEnable()
    {
        // Jika MarketBox punya event OnUpdated, subscribe agar market tahu bila storage berubah.
        // Uncomment jika MarketBox punya event public event Action OnUpdated;
        // if (storageBoxes != null) storageBoxes.OnUpdated += OnStorageUpdated;
    }

    void OnDisable()
    {
        // if (storageBoxes != null) storageBoxes.OnUpdated -= OnStorageUpdated;
    }

    void OnStorageUpdated()
    {
        // Bila storage berubah, beri tahu listener (UI).
        OnUpdated?.Invoke();
    }

    // Ambil daftar item yg ada sekarang (dibuat on-the-fly, bukan cached).
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
                    // Jika kamu ingin menampilkan count dan MarketBox tidak menyimpan count,
                    // gunakan count = 1. Jika MarketBox menyimpan count, ubah API dan ambil countnya.
                    result.Add(new ItemSlot { Item = item, Count = 1 });
                }
            }
        }

        return result;
    }

    public Vector3 GetEntrancePosition()
        => entrancePoint ? entrancePoint.position : transform.position;

    // Pilih random slot yang tersedia langsung dari storageBoxes, simpan mapping box+slot
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

    // customer meminta item tertentu -> cari di storage, set waiting if found
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

    // Commit menghapus item langsung dari storageBoxes, lalu trigger update
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

        // hapus dari storage
        storageBoxes.RemoveItem(waitingBoxIndex, waitingSlotIndex);

        // reset waiting
        waitingCustomer = null;
        waitingItem = null;
        waitingBoxIndex = -1;
        waitingSlotIndex = -1;
        sellFinished = true;

        // beri tahu UI/market listener
        OnUpdated?.Invoke();
        // bila MarketBox implement OnUpdated, storageBoxes akan memicu OnStorageUpdated via subscription
    }

    // CounterSale sama dengan Commit (kamu mungkin mau logika berbeda)
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
        // sebelum interaksi, pastikan latest storage ref
        storageBoxes = MarketBox.GetPlayerStorageBox();

        // kalau ada waiting customer/item, buka selling; else buka transfer UI
        if (waitingCustomer != null && waitingItem != null)
        {
            yield return GameController.Instance.StateMachine.PushAndWait(SellingState.i);
            yield break;
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
