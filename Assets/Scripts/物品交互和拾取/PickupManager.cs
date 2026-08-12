
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{
<<<<<<< HEAD
    [Header("�������")]
=======
    [Header("检测设置")]
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
    public float pickupRange = 2.5f;
    public float highlightRange = 2.5f;     //高亮显示范围（可独立调节）
    [Range(0, 180)]
    public float viewAngle = 60f;
    public LayerMask pickupLayer = -1;
    public LayerMask obstacleLayer = 0;
<<<<<<< HEAD
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
=======
    public KeyCode interactKey = KeyCode.F;

    [Header("提示UI")]
    public GameObject pickupHintUI;
    [Range(10, 100)]
    public float fontSize = 36f;
    public GameObject interactionHintUI;
    [Range(10, 100)]
    public float FontSize = 36f;

    [Header("高亮设置")]
    public float highlightIntensity = 1.5f;

    // 内部变量
    private Camera mainCamera;
    private Transform playerTransform;
    private PickupItem currentPickupTarget;
    private IInteractable currentInteractTarget;
    private GameObject currentInteractObject;
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
    private List<PickupItem> allPickupItems = new List<PickupItem>();
    private PickupItem lastHighlightedItem; // 记录上一个高亮物品

<<<<<<< HEAD
    // ������ɫ
=======
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
    private static readonly Color GoldColor = new Color(1f, 0.84f, 0f);
    private static readonly Color RedColor = new Color(1f, 0.2f, 0.2f);

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
<<<<<<< HEAD
            Debug.LogError("������δ�ҵ����������");
=======
            Debug.LogError("场景中未找到主摄像机！");
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
            enabled = false;
            return;
        }

        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
<<<<<<< HEAD
            Debug.LogError(this.name + "need to" + "set + playerObj");
=======
            Debug.LogError("场景中未找到Tag为'Player'的游戏对象！");
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34

        if (pickupHintUI != null)
        {
            Text hintText = pickupHintUI.GetComponent<Text>();
            if (hintText != null)
                hintText.fontSize = (int)fontSize;
            pickupHintUI.SetActive(false);
        }

        if (interactionHintUI != null)
        {
            Text hintText = interactionHintUI.GetComponent<Text>();
            if (hintText != null)
                hintText.fontSize = (int)FontSize;
            interactionHintUI.SetActive(false);
        }

        CollectPickupItems();
    }

    private void Update()
    {
<<<<<<< HEAD
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
=======
        UpdateTargetDetection();

        if (Input.GetKeyDown(interactKey))
        {
            // 交互优先
            if (currentInteractTarget != null)
            {
                currentInteractTarget.OnInteract();
                ClearInteractionTarget();
                return;
            }

            // 再拾取
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
            if (currentPickupTarget != null)
            {
                TryPickupCurrentTarget();
            }
        }
    }

    private void CollectPickupItems()
    {
        allPickupItems.Clear();
        PickupItem[] items = FindObjectsByType<PickupItem>(FindObjectsSortMode.None);
        allPickupItems.AddRange(items);
    }

    private void UpdateTargetDetection()
    {
        if (playerTransform == null || mainCamera == null) return;

<<<<<<< HEAD
        // ����Ļ���ķ�������
=======
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

<<<<<<< HEAD
        // ���߼��
        bool rayHitSomething = Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer);

        
        // �������߼�⣬���Ƿ������ĳ������
        GameObject rayHitObject = null;
        if (Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer))
=======
        ClearPickupTarget();
        ClearInteractionTarget();

        //射线检测仍用 pickupRange（性能友好）
        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
        {
            GameObject hitObject = hit.collider.gameObject;

<<<<<<< HEAD
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
=======
            //使用 highlightRange 判断是否在“高亮范围”内
            float distance = Vector3.Distance(playerTransform.position, hitObject.transform.position);
            if (distance > highlightRange) return;

            // 角度检测
            Vector3 directionToObject = hitObject.transform.position - playerTransform.position;
            float angle = Vector3.Angle(playerTransform.forward, directionToObject);
            if (angle > viewAngle / 2f) return;

            // 遮挡检测
            Vector3 dirFromCamera = hitObject.transform.position - mainCamera.transform.position;
            if (Physics.Raycast(mainCamera.transform.position, dirFromCamera,
                out RaycastHit obstacleHit, dirFromCamera.magnitude, obstacleLayer))
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
            {
                ClearPickupTarget();
                ClearInteractionTarget();
                UpdateHintUI();
                return;
            }

            // 类型判断
            IInteractable interactable = hitObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                MyInteractable myInteractable = interactable as MyInteractable;
                if (myInteractable == null || myInteractable.canInteract)
                {
                    currentInteractTarget = interactable;
                    currentInteractObject = hitObject;
                }
            }
            else
            {
                PickupItem pickupItem = hitObject.GetComponent<PickupItem>();
                if (pickupItem != null)
                {
                    currentPickupTarget = pickupItem;
                }
            }
        }

<<<<<<< HEAD
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
=======
        UpdateHighlight();
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
        UpdateHintUI();
    }

    private void UpdateHighlight()
    {
        // 取消上一个高亮
        if (lastHighlightedItem != null && lastHighlightedItem != currentPickupTarget)
        {
            SetItemHighlight(lastHighlightedItem, false);
            lastHighlightedItem = null;
        }

        // 高亮当前目标
        if (currentPickupTarget != null)
        {
            SetItemHighlight(currentPickupTarget, true);
            lastHighlightedItem = currentPickupTarget;
        }
    }

    private void ClearPickupTarget()
    {
        if (currentPickupTarget != null)
        {
            SetItemHighlight(currentPickupTarget, false);
            currentPickupTarget = null;
        }
    }

    private void ClearInteractionTarget()
    {
        currentInteractTarget = null;
        currentInteractObject = null;
    }

    private void UpdateHintUI()
    {
<<<<<<< HEAD
        // ������������ʾ
=======
        // 先隐藏所有提示
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
        if (pickupHintUI != null)
            pickupHintUI.SetActive(false);
        if (interactionHintUI != null)
            interactionHintUI.SetActive(false);

<<<<<<< HEAD
        // �����߼���������ʾ������ʰȡ��ʾ
        if (currentInteractTarget != null)
        {
            // ��ʾ������ʾ
=======
        // 交互提示优先
        if (currentInteractTarget != null)
        {
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
            if (interactionHintUI != null)
            {
                interactionHintUI.SetActive(true);
                Text hintText = interactionHintUI.GetComponent<Text>();
                if (hintText != null)
                {
                    hintText.text = currentInteractTarget.GetInteractPrompt();
                }
            }
        }
        // 拾取提示
        else if (currentPickupTarget != null)
        {
<<<<<<< HEAD
            // ��ʾʰȡ��ʾ
=======
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
            if (pickupHintUI != null)
            {
                pickupHintUI.SetActive(true);
                Text hintText = pickupHintUI.GetComponent<Text>();
                if (hintText != null)
                {
<<<<<<< HEAD
                    hintText.text = "�� F ʰȡ";
                    hintText.fontSize = (int)fontSize;
=======
                    hintText.text = "按 F 拾取";
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
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
                ? GoldColor
                : RedColor;
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
<<<<<<< HEAD
            Debug.LogError("������δ�ҵ�InventoryManager��");
=======
            Debug.LogError("场景中未找到InventoryManager！");
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
            return;
        }

        if (inv.AddItem(currentPickupTarget.itemData))
        {
            allPickupItems.Remove(currentPickupTarget);
            Destroy(currentPickupTarget.gameObject);
            ClearPickupTarget();
            UpdateHintUI();
        }
        else
        {
<<<<<<< HEAD
            Debug.Log("��Ʒ���������޷�ʰȡ");
=======
            Debug.Log("物品栏已满，无法拾取");
>>>>>>> acebff9f6dfbef81e134996332a01a6305cbaf34
        }
    }

    // 公共方法：当物品被销毁时调用
    public void OnPickupItemDestroyed(PickupItem item)
    {
        if (item == currentPickupTarget)
        {
            ClearPickupTarget();
        }
        allPickupItems.Remove(item);
    }
}