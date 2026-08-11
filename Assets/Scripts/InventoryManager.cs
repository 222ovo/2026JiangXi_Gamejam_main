using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("五个槽位引用")]
    public Slot[] slots = new Slot[5];

    [Header("当前物品列表（长度5）")]
    public List<Item> items = new List<Item>(new Item[5]);

    [Header("满背包提示UI")]
    public GameObject fullInventoryHint;   // 拖入提示UI对象（如Text）
    public float hintDuration = 2f;        // 提示显示时长（秒）

    private Coroutine hintCoroutine;       // 用于控制提示的协程

    private void Start()
    {
        RefreshUI();
        if (fullInventoryHint != null)
            fullInventoryHint.SetActive(false);  // 初始隐藏
    }

    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试添加空物品");
            return false;
        }

        // 防护：检查该物品是否已经在背包中
        if (items.Contains(item))
        {
            Debug.LogWarning($"物品 {item.name} 已经在背包中，不能重复添加");
            return false;
        }

        // 查找第一个空位
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                RefreshUI();
                Debug.Log($"物品 {item.name} 已添加到格子 {i}");
                return true;
            }
        }

        // 物品栏已满 → 显示屏幕提示
        Debug.Log("物品栏已满！");
        ShowFullInventoryHint();
        return false;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"无效的格子索引: {index}");
            return;
        }

        if (items[index] == null)
        {
            Debug.LogWarning($"格子 {index} 已经是空的");
            return;
        }

        Debug.Log($"从格子 {index} 移除了 {items[index].name}");
        items[index] = null;
        RefreshUI();
    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                return false;
        }
        return true;
    }

    public Item GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;
        return items[index];
    }

    /// <summary>
    /// 显示满背包提示，并自动消失
    /// </summary>
    private void ShowFullInventoryHint()
    {
        if (fullInventoryHint == null) return;

        // 如果之前有协程在运行，先停止它
        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        // 启动新的协程
        hintCoroutine = StartCoroutine(ShowHintCoroutine());
    }

    private IEnumerator ShowHintCoroutine()
    {
        fullInventoryHint.SetActive(true);
        yield return new WaitForSeconds(hintDuration);
        fullInventoryHint.SetActive(false);
        hintCoroutine = null;
    }

    private void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
                slots[i].SetupSlot(items[i]);
        }
    }
}