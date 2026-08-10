using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotItem;          // 当前槽位中的物品
    public Image slotImage;        // 物品图标显示

    /// <summary>
    /// 更新槽位显示（无数量）
    /// </summary>
    public void SetupSlot(Item item)
    {
        if (item == null)
        {
            // 空槽位：隐藏图标
            slotImage.gameObject.SetActive(false);
            slotItem = null;
            return;
        }

        slotItem = item;
        slotImage.sprite = item.itemIcon;
        slotImage.gameObject.SetActive(true);
    }
}
