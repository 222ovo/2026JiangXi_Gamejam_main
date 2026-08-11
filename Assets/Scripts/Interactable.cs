using UnityEngine;
using UnityEngine.Events;

public class Interactable
{
    [Header("交互设置")]
    [SerializeField] private string itemName = "可交互物品";
    [SerializeField] public bool canInteract = true;

    [Header("交互事件")]
    [SerializeField] private UnityEvent onInteract;

    public string ItemName => itemName;
    public bool CanInteract => canInteract;

    public virtual void Interact(GameObject interactor)
    {
        if (!canInteract)
        {
            return;
        }
        Debug.Log("Interact" + ItemName);
        onInteract?.Invoke();
    }

    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }
}
