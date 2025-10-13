using UnityEngine;
using UnityEngine.UI;

public class BoxMarketUI : MonoBehaviour
{
    [SerializeField] Image image;

    public void SetData(ItemBase itemBase)
    {
        image.sprite = itemBase.Icon;
        image.color = new Color(255, 255, 255, 100);
    }

    public void ClearData()
    {
        image.sprite = null;
        image.color = new Color(255, 255, 255, 0);
    }
}
