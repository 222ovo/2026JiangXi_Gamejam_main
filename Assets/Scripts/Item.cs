using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemID;
    public Sprite icon;              // ★ 物品图标（用于在物品栏中显示）
    public GameObject dropPrefab;    // ★ 丢弃时生成的掉落物预制体（可选）
    public string description;       // 物品描述（可选）

    // 你可以在这里添加其他字段，如重量、堆叠数量等
}