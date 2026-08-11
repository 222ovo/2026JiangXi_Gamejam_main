using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// 交互时调用的方法
    /// </summary>
    void OnInteract();

    /// <summary>
    /// 获取交互提示文本（用于UI显示）
    /// </summary>
    string GetInteractPrompt();
}