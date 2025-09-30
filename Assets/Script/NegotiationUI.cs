using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NegotiationUI : SelectionUI<TextSlot>
{
    [SerializeField] Button up;
    [SerializeField] Button down;
    [SerializeField] Button counter;
    [SerializeField] Text price;
    [SerializeField] Market market;
    [SerializeField] float axisThreshold = 0.2f;
    [SerializeField] float repeatSpeed = 6f;
    float navTimer = 0f;
    int itemCount = 0;
    int lastVerticalIndex = 0;
    int currentPrice;

    void Awake()
    {
        SetSelectionSettings(SelectionType.SpecialGrid, 2);
        var slots = GetComponentsInChildren<TextSlot>(true).ToList();
        SetItems(slots);
        currentPrice = market.SelectedItem.NegoPrice;

        OnSpecialVertical += (index, dir) =>
        {
            if (index == 1) // hanya di index 1 vertical dipakai adjust harga
            {
                currentPrice = Mathf.Max(0, currentPrice + (dir > 0 ? +10 : -10));
                price.text = currentPrice.ToString();
                return true; // KONSUMSI → seleksi tidak pindah
            }
            return false; // tidak dipakai → seleksi pindah dalam row (±1)
        };
    }

    void Start()
    {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
        SetupButtons();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void SetupButtons()
    {
        var items = GetComponentsInChildren<TextSlot>(true).ToList();
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            items[i].OnClick = () =>
            {
                selectedItem = Mathf.Clamp(index, 0, itemCount - 1);
                if (selectedItem != 2) lastVerticalIndex = selectedItem;
                UpdateSelectionInUI();
                OnConfirmButton();
            };
        }
    }
    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }
    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();
        
        price.text = "" + currentPrice;
    }
}
