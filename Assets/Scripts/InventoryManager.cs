using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    [Header("槽位引用")]
    public Slot[] slots = new Slot[5];

    [Header("物品列表（长度5）")]
    public List<Item> items = new List<Item>(new Item[5]);

    [Header("满背包提示UI")]
    public GameObject fullInventoryHint;
    public float hintDuration = 2f;

    [Header("选中相关设置")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public KeyCode dropKey = KeyCode.G;

    private int selectedIndex = 0;
    private Coroutine hintCoroutine;

    private void Start()
    {
        // 自动查找提示UI
        if (fullInventoryHint == null)
            fullInventoryHint = GameObject.Find("FullInventoryHint");

        if (fullInventoryHint != null)
            fullInventoryHint.SetActive(false);

        // 初始化物品列表
        if (items.Count != slots.Length)
        {
            items = new List<Item>(new Item[slots.Length]);
        }

        RefreshUI();
        SelectSlot(0); // 默认选中第一个
    }

    private void Update()
    {
        // 鼠标滚轮切换
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? -1 : 1;
            int newIndex = selectedIndex + direction;

            if (newIndex < 0) newIndex = slots.Length - 1;
            if (newIndex >= slots.Length) newIndex = 0;

            SelectSlot(newIndex);
        }

        // 数字键 1~5 切换
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SelectSlot(i);
                break;
            }
        }

        // 按 G 丢弃当前选中物品
        if (Input.GetKeyDown(dropKey))
        {
            DropCurrentItem();
        }
    }

    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试添加空物品");
            return false;
        }

        //if (items.Contains(item))
        //{
        //    Debug.LogWarning($"物品 {item.name} 已经在背包中");
        //    return false;
        //}
        if (items.Any(i => i != null && i.itemID == item.itemID))
        {
            Debug.LogWarning($"物品 {item.name} 数量已达上限");
            return false;
        }


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

        Debug.Log("物品栏已满！");
        ShowFullInventoryHint();
        return false;
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if (selectedIndex == index) return;

        // 取消原选中格子的高亮
        if (slots[selectedIndex] != null)
            slots[selectedIndex].SetHighlight(false);

        // 选中新格子
        selectedIndex = index;
        if (slots[selectedIndex] != null)
            slots[selectedIndex].SetHighlight(true);
    }
    private void DropCurrentItem()
    {
        if (items[selectedIndex] == null)
        {
            Debug.Log("当前选中的物品栏是空的，无法丢弃");
            return;
        }

        Item droppedItem = items[selectedIndex];
        Debug.Log($"丢弃了物品: {droppedItem.name}");

        // ★ 使用 item.dropPrefab 生成掉落物
        DropItemInWorld(droppedItem);

        items[selectedIndex] = null;
        RefreshUI();
    }

    private void DropItemInWorld(Item item)
    {
        // 如果有 dropPrefab，则在玩家前方生成掉落物
        if (item.dropPrefab != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 dropPosition = player.transform.position + player.transform.forward * 2f + Vector3.up * 0.5f;
                Instantiate(item.dropPrefab, dropPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log($"物品 {item.name} 没有设置 dropPrefab，未生成掉落物");
        }
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if (items[index] == null) return;

        Debug.Log($"从格子 {index} 移除了 {items[index].name}");
        items[index] = null;
        RefreshUI();
    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) return false;
        }
        return true;
    }

    public Item GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count)
            return null;
        return items[selectedIndex];
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    private void ShowFullInventoryHint()
    {
        if (fullInventoryHint == null)
        {
            Debug.LogWarning("fullInventoryHint 未赋值");
            return;
        }

        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

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
            {
                slots[i].SetupSlot(items[i]);
                slots[i].SetHighlight(i == selectedIndex);  // 只控制边框
            }
        }
    }
}