
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{
    [Header("检测设置")]
    public float pickupRange = 2.5f;
    public float highlightRange = 2.5f;     //高亮显示范围（可独立调节）
    [Range(0, 180)]
    public float viewAngle = 60f;
    public LayerMask pickupLayer = -1;
    public LayerMask obstacleLayer = 0;
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
    private List<PickupItem> allPickupItems = new List<PickupItem>();
    private PickupItem lastHighlightedItem; // 记录上一个高亮物品

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

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        ClearPickupTarget();
        ClearInteractionTarget();

        //射线检测仍用 pickupRange（性能友好）
        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

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

        UpdateHighlight();
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
        // 先隐藏所有提示
        if (pickupHintUI != null)
            pickupHintUI.SetActive(false);
        if (interactionHintUI != null)
            interactionHintUI.SetActive(false);

        // 交互提示优先
        if (currentInteractTarget != null)
        {
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
            if (pickupHintUI != null)
            {
                pickupHintUI.SetActive(true);
                Text hintText = pickupHintUI.GetComponent<Text>();
                if (hintText != null)
                {
                    hintText.text = "按 F 拾取";
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
            Debug.LogError("场景中未找到InventoryManager！");
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
            Debug.Log("物品栏已满，无法拾取");
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