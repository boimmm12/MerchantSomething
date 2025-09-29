using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Button button;
    Image bgImage;
    void Awake()
    {
        bgImage = GetComponent<Image>();
    }

    private Color originalColor;

    public Action OnClick { get; set; }

    public void Init()
    {
        originalColor = bgImage.color;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClick?.Invoke());
        }
    }

    public void Clear()
    {
        bgImage.color = originalColor;
        if (button != null)
            button.onClick.RemoveAllListeners();
    }

    public void OnSelectionChanged(bool selected)
    {
        bgImage.color = selected ? Color.blue : originalColor;
        Debug.Log("ganti");
    }
}
