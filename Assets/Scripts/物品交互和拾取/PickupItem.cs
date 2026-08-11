using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemData;
    public HighlightType highlightType = HighlightType.Gold;

    // 新增：标记此物品是否允许交互（有些物品既可以拾取也可以交互）
    //[Header("交互设置")]
    //public bool isInteractable = false;

    public enum HighlightType
    {
        Gold,
        Red
    }
}