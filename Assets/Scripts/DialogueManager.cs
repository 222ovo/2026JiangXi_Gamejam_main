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
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue();

        speakerNameText.text = GetDisplaySpeakerName(line.speaker);
        dialogueContentText.text = line.content;
    }

    private string GetDisplaySpeakerName(string speaker)
    {
        if (speaker == "独白")
        {
            return "内心独白";
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