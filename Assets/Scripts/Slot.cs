// Slot.cs 修改后
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image iconImage;      // 物品图标
    public Image borderImage;    // 边框，默认隐藏，选中时显示
    public Image backgroundImage; // 新增：背景图片，用于颜色变化

    public Item currentItem;

    [Header("颜色设置")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private void Awake()
    {
        // 自动查找图标
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();

        // 自动查找背景（假设背景是第一个Image组件）
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        // 边框默认隐藏
        if (borderImage != null)
            borderImage.enabled = false;

        // 初始化背景颜色
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    /// <summary>
    /// 设置物品显示
    /// </summary>
    public void SetupSlot(Item item)
    {
        currentItem = item;

        if (iconImage == null) return;

        if (item != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            //iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    /// <summary>
    /// 设置高亮状态，同时改变边框和背景颜色
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        // 控制边框显示
        if (borderImage != null)
        {
            borderImage.enabled = highlight;
        }

        // 控制背景颜色
        if (backgroundImage != null)
        {
            backgroundImage.color = highlight ? selectedColor : normalColor;
        }
    }
}