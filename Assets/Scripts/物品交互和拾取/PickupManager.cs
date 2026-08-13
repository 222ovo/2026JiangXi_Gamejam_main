using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{
    [Header("�������")]
    public float pickupRange = 2.5f;
    [Range(0, 180)]
    public float viewAngle = 60f;
    public LayerMask pickupLayer = -1;
    public LayerMask obstacleLayer = 0;
    public KeyCode interactKey = KeyCode.F;      // ����/ʰȡ����
    [SerializeField] private GameObject playerObj;

    //[Header("ʰȡ��ʾUI")]
    //public GameObject pickupHintUI;
    //[Range(10, 100)]
    //public float fontSize = 36f;


    [Header("��ʾUI����һ����ʰȡ���ڶ����ǽ���")]
    public GameObject pickupHintUI;        // ʰȡ��ʾ���硰��Fʰȡ����
    [Range(10, 100)]
    public float fontSize = 36f;
    public GameObject interactionHintUI;   // ������ʾ���硰��F���š���
    [Range(10, 100)]
    public float FontSize = 36f;


    [Header("��������")]
    public float highlightIntensity = 1.5f;

    // �ڲ�����
    private Camera mainCamera;
    private Transform playerTransform;
    private Text hintText;
    private PickupItem currentPickupTarget;       // ��ǰ������Ŀ����Ʒ����ʰȡ��
    private IInteractable currentInteractTarget;  // ��ǰ�ɽ�����Ŀ��
    private GameObject currentInteractObject;     // ��ǰ�ɽ���Ŀ���Ӧ��GameObject
    private List<PickupItem> allPickupItems = new List<PickupItem>();

    // ������ɫ
    private static readonly Color GoldColor = new Color(1f, 0.84f, 0f);
    private static readonly Color RedColor = new Color(1f, 0.2f, 0.2f);

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("������δ�ҵ����������");
            enabled = false;
            return;
        }

        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError(this.name + "need to" + "set + playerObj");

        if (pickupHintUI != null)
        {
            hintText = pickupHintUI.GetComponent<Text>();
            if (hintText != null)
                hintText.fontSize = (int)fontSize;
            pickupHintUI.SetActive(false);
        }

        CollectPickupItems();
    }

    private void Update()
    {
        // ÿ֡��⵱ǰĿ��
        UpdateTargetDetection();

        // ����F��ʱ���ȳ��Խ������ٳ���ʰȡ
        if (Input.GetKeyDown(interactKey))
        {
            // ���Ƚ���
            if (currentInteractTarget != null)
            {
                currentInteractTarget.OnInteract();
                // �����������Ʒ�����ٻ���ã���Ҫ����
                if (currentInteractObject == null || !currentInteractObject.activeInHierarchy)
                {
                    currentInteractTarget = null;
                    currentInteractObject = null;
                }
                return; // ִ�н�������ִ��ʰȡ
            }

            // ���û�н���Ŀ�꣬����ʰȡ
            if (currentPickupTarget != null)
            {
                TryPickupCurrentTarget();
            }
        }
    }

    private void CollectPickupItems()
    {
        PickupItem[] items = FindObjectsByType<PickupItem>(FindObjectsSortMode.None);
        allPickupItems.Clear();
        allPickupItems.AddRange(items);
    }

    private void UpdateTargetDetection()
    {
        if (playerTransform == null) return;

        // ����Ļ���ķ�������
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit rayHit;

        // ���߼��
        bool rayHitSomething = Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer);

        
        // �������߼�⣬���Ƿ������ĳ������
        GameObject rayHitObject = null;
        if (Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer))
        {
            rayHitObject = rayHit.collider.gameObject;
        }

        // �������п�ʰȡ��Ʒ���ҵ����������������
        PickupItem bestPickupTarget = null;
        float bestPickupDistance = Mathf.Infinity;

        // �������пɽ�����Ʒ��ͨ����ⳡ��������IInteractable��
        IInteractable bestInteractTarget = null;
        GameObject bestInteractObject = null;
        float bestInteractDistance = Mathf.Infinity;

        // ������п�ʰȡ��Ʒ
        foreach (PickupItem item in allPickupItems)
        {
            if (item == null || !item.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(playerTransform.position, item.transform.position);
            if (distance > pickupRange) continue;

            Vector3 directionToItem = item.transform.position - playerTransform.position;
            float angle = Vector3.Angle(playerTransform.forward, directionToItem);
            if (angle > viewAngle / 2f) continue;

            // ���߼�⣺����Ƿ����ϰ����ڵ�
            Vector3 dirFromCamera = item.transform.position - mainCamera.transform.position;
            RaycastHit obstacleHit;
            if (Physics.Raycast(mainCamera.transform.position, dirFromCamera, out obstacleHit, dirFromCamera.magnitude, obstacleLayer))
                continue;

            if (rayHitObject != null && item.gameObject == rayHitObject && distance < bestPickupDistance)
            {
                bestPickupTarget = item;
                bestPickupDistance = distance;
            }
            else if (rayHitObject == null && distance < bestPickupDistance)
            {
                bestPickupTarget = item;
                bestPickupDistance = distance;
            }
        }

        // ������пɽ�������
        // ע�⣺����ʹ��FindObjectsByTypeÿ֡���ң����ܿ�����Ӱ�졣������Start/Addʱ�����б�
        IInteractable[] allInteractables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IInteractable>()
            .ToArray();

        foreach (IInteractable interactable in allInteractables)
        {
            if (interactable is Interactable interactableComp && !interactableComp.canInteract)
                continue;

            MonoBehaviour mb = interactable as MonoBehaviour;
            if (mb == null || !mb.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(playerTransform.position, mb.transform.position);
            if (distance > pickupRange) continue;

            Vector3 directionToItem = mb.transform.position - playerTransform.position;
            float angle = Vector3.Angle(playerTransform.forward, directionToItem);
            if (angle > viewAngle / 2f) continue;

            Vector3 dirFromCamera = mb.transform.position - mainCamera.transform.position;
            RaycastHit obstacleHit;
            if (Physics.Raycast(mainCamera.transform.position, dirFromCamera, out obstacleHit, dirFromCamera.magnitude, obstacleLayer))
                continue;

            if (rayHitObject != null && mb.gameObject == rayHitObject && distance < bestInteractDistance)
            {
                bestInteractTarget = interactable;
                bestInteractObject = mb.gameObject;
                bestInteractDistance = distance;
            }
            else if (rayHitObject == null && distance < bestInteractDistance)
            {
                bestInteractTarget = interactable;
                bestInteractObject = mb.gameObject;
                bestInteractDistance = distance;
            }
        }

        // ���½���Ŀ��
        if (bestInteractTarget != currentInteractTarget)
        {
            currentInteractTarget = bestInteractTarget;
            currentInteractObject = bestInteractObject;
        }

        // ����ʰȡĿ��
        if (bestPickupTarget != currentPickupTarget)
        {
            if (currentPickupTarget != null)
                SetItemHighlight(currentPickupTarget, false);

            currentPickupTarget = bestPickupTarget;
            if (currentPickupTarget != null)
                SetItemHighlight(currentPickupTarget, true);
        }

        // ������ʾUI
        UpdateHintUI();
    }

    private void UpdateHintUI()
    {
        // ������������ʾ
        if (pickupHintUI != null)
            pickupHintUI.SetActive(false);
        if (interactionHintUI != null)
            interactionHintUI.SetActive(false);

        // �����߼���������ʾ������ʰȡ��ʾ
        if (currentInteractTarget != null)
        {
            // ��ʾ������ʾ
            if (interactionHintUI != null)
            {
                interactionHintUI.SetActive(true);
                Text hintText = interactionHintUI.GetComponent<Text>();
                if (hintText != null)
                {
                    hintText.text = currentInteractTarget.GetInteractPrompt();
                    hintText.fontSize = (int)fontSize;
                }
            }
        }
        else if (currentPickupTarget != null)
        {
            // ��ʾʰȡ��ʾ
            if (pickupHintUI != null)
            {
                pickupHintUI.SetActive(true);
                Text hintText = pickupHintUI.GetComponent<Text>();
                if (hintText != null)
                {
                    hintText.text = "�� F ʰȡ";
                    hintText.fontSize = (int)fontSize;
                }
            }
        }
    }

    private void SetItemHighlight(PickupItem item, bool highlight)
    {
        if (item == null) return;

        Renderer rend = item.GetComponent<Renderer>();
        if (rend == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        if (highlight)
        {
            Color emissionColor = (item.highlightType == PickupItem.HighlightType.Gold)
                ? new Color(1f, 0.84f, 0f)
                : new Color(1f, 0.2f, 0.2f);
            mpb.SetColor("_EmissionColor", emissionColor * highlightIntensity);
        }
        else
        {
            mpb.SetColor("_EmissionColor", Color.black);
        }

        rend.SetPropertyBlock(mpb);
    }
    private void TryPickupCurrentTarget()
    {
        if (currentPickupTarget == null) return;

        InventoryManager inv = FindFirstObjectByType<InventoryManager>();
        if (inv == null)
        {
            Debug.LogError("������δ�ҵ�InventoryManager��");
            return;
        }

        if (inv.AddItem(currentPickupTarget.itemData))
        {
            allPickupItems.Remove(currentPickupTarget);
            Destroy(currentPickupTarget.gameObject);
            currentPickupTarget = null;
            UpdateHintUI();
        }
        else
        {
            Debug.Log("��Ʒ���������޷�ʰȡ");
        }
    }
}