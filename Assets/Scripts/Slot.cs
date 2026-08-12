using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;        // 物品图标显示
    public Image backgroundImage;  // 背景（用于高亮颜色变化）

    private Item currentItem;

    private void Awake()
    {
        // 自动查找组件
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
    }

    /// <summary>
    /// 设置格子显示的物品
    /// </summary>
    public void SetupSlot(Item item)
    {
        currentItem = item;

        if (iconImage == null) return;

        // ★ 使用 item.icon 显示物品图标
        if (item != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    /// <summary>
    /// 设置高亮状态
    /// </summary>
    public void SetHighlight(bool highlight, Color highlightColor)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = highlight ? highlightColor : Color.white;
        }
    }

    /// <summary>
    /// 获取当前格子中的物品
    /// </summary>
    public Item GetItem()
    {
        return currentItem;
    }
}