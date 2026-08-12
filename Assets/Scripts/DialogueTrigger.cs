using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset dialogueText;

    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            dialogueManager.StartDialogue(dialogueText);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            print("Player entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            print("Player exit");
        }
    }
}