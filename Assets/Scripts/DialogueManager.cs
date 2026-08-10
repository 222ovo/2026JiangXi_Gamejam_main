using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueContentText;

    [Header("输入")]
    [SerializeField] private KeyCode nextKey = KeyCode.Space;
    
    [Header("字幕弹出")]
    private Coroutine typingCoroutine;
    private float typingSpeed = 0.05f;
    
    private readonly Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isDialoguePlaying;

    private void Start()
    {
        HideDialogue();
    }

    private void Update()
    {
        if (!isDialoguePlaying)
        {
            return;
        }

        if (Input.GetKeyDown(nextKey) || Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(TextAsset dialogueText)
    {
        if (dialogueText == null)
        {
            Debug.LogWarning("没有传入对话文本。");
            return;
        }

        dialogueQueue.Clear();

        string[] lines = dialogueText.text.Split('\n');

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length < 2)
            {
                Debug.LogWarning("对话格式错误：" + line);
                continue;
            }

            DialogueLine dialogueLine = new DialogueLine
            {
                speaker = parts[0].Trim(),
                content = parts[1].Trim()
            };

            dialogueQueue.Enqueue(dialogueLine);
        }

        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        // 如果当前正在打字，点击时直接跳过动画显示全文（防止重复点击冲突）
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            // 如果有上一句没打完的字，直接显示完整
            if (dialogueQueue.Count > 0)
            {
                DialogueLine line = dialogueQueue.Peek(); // 查看但不移除
                dialogueContentText.text = line.content;
            }
        }

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine _line = dialogueQueue.Dequeue();
    
        speakerNameText.text = GetDisplaySpeakerName(_line.speaker);
    
        // 启动协程来实现逐字显示
        typingCoroutine = StartCoroutine(TypeText(_line.content));
    }
    
    private IEnumerator TypeText(string content)
    {
        dialogueContentText.text = ""; // 清空当前文本
    
        foreach (var character in content)
        {
            dialogueContentText.text += character;
            yield return new WaitForSeconds(typingSpeed); // 等待一段时间再显示下一个字
        }

        typingCoroutine = null; // 打字完成，重置协程变量
    }

    private string GetDisplaySpeakerName(string speaker)
    {
        if (speaker == "独白")
        {
            return "我";
        }

        return speaker;
    }

    private void EndDialogue()
    {
        isDialoguePlaying = false;
        HideDialogue();
    }

    private void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string content;
}