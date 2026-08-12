using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{
    [Header("检测设置")]
    public float pickupRange = 2.5f;
    [Range(0, 180)]
    public float viewAngle = 60f;
    public LayerMask pickupLayer = -1;
    public LayerMask obstacleLayer = 0;
    public KeyCode interactKey = KeyCode.F;      // 交互/拾取按键

    //[Header("拾取提示UI")]
    //public GameObject pickupHintUI;
    //[Range(10, 100)]
    //public float fontSize = 36f;


    [Header("提示UI：第一个是拾取，第二个是交互")]
    public GameObject pickupHintUI;        // 拾取提示（如“按F拾取”）
    [Range(10, 100)]
    public float fontSize = 36f;
    public GameObject interactionHintUI;   // 交互提示（如“按F开门”）
    [Range(10, 100)]
    public float FontSize = 36f;


    [Header("高亮设置")]
    public float highlightIntensity = 1.5f;

    // 内部变量
    private Camera mainCamera;
    private Transform playerTransform;
    private Text hintText;
    private PickupItem currentPickupTarget;       // 当前高亮的目标物品（可拾取）
    private IInteractable currentInteractTarget;  // 当前可交互的目标
    private GameObject currentInteractObject;     // 当前可交互目标对应的GameObject
    private List<PickupItem> allPickupItems = new List<PickupItem>();

    // 高亮颜色
    private static readonly Color GoldColor = new Color(1f, 0.84f, 0f);
    private static readonly Color RedColor = new Color(1f, 0.2f, 0.2f);

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("场景中未找到主摄像机！");
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("场景中未找到Tag为'Player'的游戏对象！");

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
        // 每帧检测当前目标
        UpdateTargetDetection();

        // 按下F键时，先尝试交互，再尝试拾取
        if (Input.GetKeyDown(interactKey))
        {
            // 优先交互
            if (currentInteractTarget != null)
            {
                currentInteractTarget.OnInteract();
                // 交互后如果物品被销毁或禁用，需要清理
                if (currentInteractObject == null || !currentInteractObject.activeInHierarchy)
                {
                    currentInteractTarget = null;
                    currentInteractObject = null;
                }
                return; // 执行交互后不再执行拾取
            }

            // 如果没有交互目标，尝试拾取
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

        // 从屏幕中心发射射线
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit rayHit;

        // 射线检测
        bool rayHitSomething = Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer);

        
        // 先用射线检测，看是否击中了某个物体
        GameObject rayHitObject = null;
        if (Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer))
        {
            rayHitObject = rayHit.collider.gameObject;
        }

        // 遍历所有可拾取物品，找到最近且满足条件的
        PickupItem bestPickupTarget = null;
        float bestPickupDistance = Mathf.Infinity;

        // 遍历所有可交互物品（通过检测场景中所有IInteractable）
        IInteractable bestInteractTarget = null;
        GameObject bestInteractObject = null;
        float bestInteractDistance = Mathf.Infinity;

        // 检测所有可拾取物品
        foreach (PickupItem item in allPickupItems)
        {
            if (item == null || !item.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(playerTransform.position, item.transform.position);
            if (distance > pickupRange) continue;

            Vector3 directionToItem = item.transform.position - playerTransform.position;
            float angle = Vector3.Angle(playerTransform.forward, directionToItem);
            if (angle > viewAngle / 2f) continue;

            // 射线检测：检查是否有障碍物遮挡
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

        // 检测所有可交互物体
        // 注意：这里使用FindObjectsByType每帧查找，性能可能受影响。建议在Start/Add时缓存列表。
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

        // 更新交互目标
        if (bestInteractTarget != currentInteractTarget)
        {
            currentInteractTarget = bestInteractTarget;
            currentInteractObject = bestInteractObject;
        }

        // 更新拾取目标
        if (bestPickupTarget != currentPickupTarget)
        {
            if (currentPickupTarget != null)
                SetItemHighlight(currentPickupTarget, false);

            currentPickupTarget = bestPickupTarget;
            if (currentPickupTarget != null)
                SetItemHighlight(currentPickupTarget, true);
        }

        // 更新提示UI
        UpdateHintUI();
    }

    private void UpdateHintUI()
    {
        // 先隐藏所有提示
        if (pickupHintUI != null)
            pickupHintUI.SetActive(false);
        if (interactionHintUI != null)
            interactionHintUI.SetActive(false);

        // 互斥逻辑：交互提示优先于拾取提示
        if (currentInteractTarget != null)
        {
            // 显示交互提示
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
            // 显示拾取提示
            if (pickupHintUI != null)
            {
                pickupHintUI.SetActive(true);
                Text hintText = pickupHintUI.GetComponent<Text>();
                if (hintText != null)
                {
                    hintText.text = "按 F 拾取";
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
            Debug.LogError("场景中未找到InventoryManager！");
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
            Debug.Log("物品栏已满，无法拾取");
        }
    }
}