using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemData;
    public HighlightType highlightType = HighlightType.Gold;

    // 此脚本仅作为数据标记，高亮和提示由PickupManager统一管理
    // 不需要额外逻辑

    public enum HighlightType
    {
        Gold,
        Red
    }
}