using UnityEngine;
using UnityEngine.Events;

public class MyInteractable : MonoBehaviour, IInteractable
{
    [Header("交互开关")]
    public bool canInteract = true;          // 勾选表示可交互，取消勾选表示不可交互

    [Header("交互提示")]
    public string interactPrompt = "按 F 交互";

    [Header("交互提示UI")]
    public GameObject interactionHintUI;
    [Range(10, 100)]
    public float fontSize = 36f;



    [Header("交互事件")]
    public UnityEvent onInteract;            // 在Inspector中绑定交互时触发的逻辑

    // 实现 IInteractable 接口
    public void OnInteract()
    {
        if (!canInteract)
        {
            Debug.Log($"{gameObject.name} 当前不可交互");
            return;
        }

        // 调用Inspector中绑定的事件
        onInteract?.Invoke();
    }

    public string GetInteractPrompt()
    {
        return canInteract ? interactPrompt : "";
    }

    // 公开方法：在代码中动态启用/禁用交互
    public void SetInteractable(bool value)
    {
        canInteract = value;
    }
}