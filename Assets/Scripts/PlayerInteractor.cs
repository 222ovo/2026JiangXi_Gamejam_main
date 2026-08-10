using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("检测设置")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask interactLayer;
    
    private Interactable currentItem;
    
    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }
    
    private void Update()
    {
        CheckItem();
    
        if (currentItem != null && Input.GetKeyDown(interactKey))
        {
            currentItem.Interact(gameObject);
        }
    }
    
    private void CheckItem()
    {
        currentItem = null;
    
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            currentItem = hit.collider.GetComponent<Interactable>();
    
            if (currentItem == null)
            {
                currentItem = hit.collider.GetComponentInParent<Interactable>();
            }
    
            if (currentItem != null && !currentItem.CanInteract)
            {
                currentItem = null;
            }
        }
    }
}