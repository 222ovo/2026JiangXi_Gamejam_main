using UnityEngine;
using System.Collections.Generic;


public class InventoryManager : MonoBehaviour
{
    [Header("五个槽位引用")]
    public Slot[] slots = new Slot[5];

    [Header("当前物品列表（长度5）")]
    public List<Item> items = new List<Item>(new Item[5]);

    private void Start()
    {
        RefreshUI();
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                RefreshUI();
                return true;
            }
        }
        Debug.Log("物品栏已满！");
        return false;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        items[index] = null;
        RefreshUI();
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
