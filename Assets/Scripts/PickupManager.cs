using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PickupManager : MonoBehaviour
{
    [Header("检测设置")]
    public float pickupRange = 5f;                 // 最大拾取距离
    [Range(0, 180)]
    public float viewAngle = 60f;                  // 视角角度（完整角度）
    public LayerMask pickupLayer = -1;             // 可拾取物品所在的层级（-1表示所有层级）
    public LayerMask obstacleLayer = 0;            // 障碍物层级（用于射线遮挡检测）
    public KeyCode pickupKey = KeyCode.F;          // 拾取按键

    [Header("拾取提示UI")]
    public GameObject pickupHintUI;                // 提示UI对象
    [Range(10, 100)]
    public float fontSize = 36f;                   // 字体大小

    [Header("高亮设置")]
    public float highlightIntensity = 1.5f;         // 高亮强度

    // 内部变量
    private Camera mainCamera;
    private Transform playerTransform;
    private Text hintText;
    private PickupItem currentTarget;               // 当前高亮的目标物品
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

        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("场景中未找到Tag为'Player'的游戏对象！");

        // 初始化提示UI
        if (pickupHintUI != null)
        {
            hintText = pickupHintUI.GetComponent<Text>();
            if (hintText != null)
                hintText.fontSize = (int)fontSize;
            pickupHintUI.SetActive(false);
        }

        // 收集所有可拾取物品
        CollectPickupItems();
    }

    private void Update()
    {
        // 检测并更新当前目标
        UpdateTargetDetection();

        // 拾取操作
        if (currentTarget != null && Input.GetKeyDown(pickupKey))
        {
            TryPickupCurrentTarget();
        }
    }

    /// <summary>
    /// 收集场景中所有可拾取物品
    /// </summary>
    private void CollectPickupItems()
    {
        PickupItem[] items = FindObjectsByType<PickupItem>(FindObjectsSortMode.None);
        allPickupItems.Clear();
        allPickupItems.AddRange(items);
    }

    /// <summary>
    /// 检测所有物品，找到最近的有效目标
    /// </summary>
    private void UpdateTargetDetection()
    {
        // 如果没有玩家引用，直接返回
        if (playerTransform == null) return;

        // 从屏幕中心发射射线
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit rayHit;

        // 先用射线检测，看是否击中了某个可拾取物品
        GameObject rayHitObject = null;
        if (Physics.Raycast(ray, out rayHit, pickupRange, pickupLayer))
        {
            // 检查射线击中的物体是否有PickupItem组件
            if (rayHit.collider.GetComponent<PickupItem>() != null)
            {
                rayHitObject = rayHit.collider.gameObject;
            }
        }

        // 遍历所有可拾取物品，找到最近且满足条件的
        PickupItem bestTarget = null;
        float bestDistance = Mathf.Infinity;

        foreach (PickupItem item in allPickupItems)
        {
            if (item == null || !item.gameObject.activeInHierarchy) continue;

            // 计算距离
            float distance = Vector3.Distance(playerTransform.position, item.transform.position);
            if (distance > pickupRange) continue;

            // 计算角度
            Vector3 directionToItem = item.transform.position - playerTransform.position;
            float angle = Vector3.Angle(playerTransform.forward, directionToItem);
            if (angle > viewAngle / 2f) continue;

            // 射线检测：检查是否有障碍物遮挡
            Vector3 directionToItemFromCamera = item.transform.position - mainCamera.transform.position;
            RaycastHit obstacleHit;
            if (Physics.Raycast(mainCamera.transform.position, directionToItemFromCamera, out obstacleHit, directionToItemFromCamera.magnitude, obstacleLayer))
            {
                // 有障碍物遮挡，跳过
                continue;
            }

            // 如果射线检测到物体，优先选择射线击中的物体
            if (rayHitObject != null && item.gameObject == rayHitObject)
            {
                // 射线击中的物体具有最高优先级
                if (distance < bestDistance)
                {
                    bestTarget = item;
                    bestDistance = distance;
                }
            }
            else if (rayHitObject == null)
            {
                // 没有射线击中的物体，选择距离最近的
                if (distance < bestDistance)
                {
                    bestTarget = item;
                    bestDistance = distance;
                }
            }
        }

        // 更新高亮和提示
        if (bestTarget != currentTarget)
        {
            // 取消旧目标的高亮和提示
            if (currentTarget != null)
            {
                SetItemHighlight(currentTarget, false);
            }

            // 设置新目标的高亮和提示
            currentTarget = bestTarget;
            if (currentTarget != null)
            {
                SetItemHighlight(currentTarget, true);
                ShowPickupHint(true);
            }
            else
            {
                ShowPickupHint(false);
            }
        }
        else if (bestTarget == null && currentTarget == null)
        {
            // 没有目标，确保提示隐藏
            ShowPickupHint(false);
        }
    }

    /// <summary>
    /// 设置物品的高亮状态
    /// </summary>
    private void SetItemHighlight(PickupItem item, bool highlight)
    {
        if (item == null) return;

        Renderer rend = item.GetComponent<Renderer>();
        if (rend == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        if (highlight)
        {
            Color emissionColor = (item.highlightType == PickupItem.HighlightType.Gold) ? GoldColor : RedColor;
            mpb.SetColor("_EmissionColor", emissionColor * highlightIntensity);
        }
        else
        {
            mpb.SetColor("_EmissionColor", Color.black);
        }

        rend.SetPropertyBlock(mpb);
    }

    /// <summary>
    /// 显示/隐藏拾取提示
    /// </summary>
    private void ShowPickupHint(bool show)
    {
        if (pickupHintUI != null)
        {
            pickupHintUI.SetActive(show);
            if (hintText != null)
                hintText.fontSize = (int)fontSize;
        }
    }

    /// <summary>
    /// 拾取当前目标
    /// </summary>
    private void TryPickupCurrentTarget()
    {
        if (currentTarget == null) return;

        InventoryManager inv = FindFirstObjectByType<InventoryManager>();
        if (inv == null)
        {
            Debug.LogError("场景中未找到InventoryManager！");
            return;
        }

        if (inv.AddItem(currentTarget.itemData))
        {
            // 移除目标物体
            Destroy(currentTarget.gameObject);
            // 从列表中移除
            allPickupItems.Remove(currentTarget);
            // 清除当前目标
            currentTarget = null;
            // 隐藏提示
            ShowPickupHint(false);
        }
        else
        {
            Debug.Log("物品栏已满，无法拾取");
        }
    }

    // 可视化调试
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
            else
                return;
        }

        // 绘制距离范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, pickupRange);

        // 绘制视角范围（扇形）
        Vector3 forward = playerTransform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(playerTransform.position, forward * pickupRange);
        Gizmos.DrawRay(playerTransform.position, right * pickupRange);
        Gizmos.DrawRay(playerTransform.position, left * pickupRange);

        // 绘制扇形弧线
        int segments = 20;
        float stepAngle = viewAngle / segments;
        Vector3 prevPoint = playerTransform.position + left * pickupRange;
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -viewAngle / 2f + i * stepAngle;
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;
            Vector3 currentPoint = playerTransform.position + direction * pickupRange;
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }
}