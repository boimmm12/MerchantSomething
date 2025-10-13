using UnityEngine;
using UnityEngine.UI;
public class BoxInventoryUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Text countText;
    public void SetData(ItemBase itemBase, int count = 1)
    {
        image.sprite = itemBase.Icon;
        image.color = new Color(255, 255, 255, 100);
        countText.text = count > 1 ? count.ToString() : "";
    }

    public void ClearData()
    {
        image.sprite = null;
        image.color = new Color(255, 255, 255, 0);
        countText.text = "";
    }
}
