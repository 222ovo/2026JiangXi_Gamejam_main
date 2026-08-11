using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("交互设置")]
    public string interactPrompt = "按 F 交互";   // 交互提示文本

    // 实现 IInteractable 接口
    public virtual void OnInteract()
    {
        // 子类重写此方法，实现具体的交互逻辑
        Debug.Log($"与 {gameObject.name} 进行了交互");
    }

    public string GetInteractPrompt()
    {
        return interactPrompt;
    }
}