using System;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] private Text text;
    [SerializeField] private Button button;

    private Color originalColor;

    public Action OnClick { get; set; }

    public void Init()
    {
        originalColor = text.color;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClick?.Invoke());
        }
    }

    public void Clear()
    {
        text.color = originalColor;
        if (button != null)
            button.onClick.RemoveAllListeners();
    }

    public void OnSelectionChanged(bool selected)
    {
        text.color = selected ? Color.blue : originalColor;
    }

    public void SetText(string s)
    {
        text.text = s;
    }
}
