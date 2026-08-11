using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    [Header("物品数据")]
    public Item itemData;

    [Header("高亮类型")]
    public HighlightType highlightType = HighlightType.Gold;

    [Header("高亮强度")]
    [Range(0.1f, 3f)]
    public float highlightIntensity = 1.5f;

    [Header("拾取检测设置")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupRange = 5f;                 // 最大拾取距离
    [Range(0, 180)]
    public float viewAngle = 60f;                  // 视角角度（半角，即左右各30°）
    public LayerMask playerLayer;                  // 玩家所在的层级

    [Header("拾取提示UI")]
    public GameObject pickupHintUI;
    [Range(10, 100)]
    public float fontSize = 36f;

    // 内部变量
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private bool hasEmission;
    private Text hintText;
    private Transform playerTransform;             // 玩家Transform引用
    private bool isPlayerInView = false;           // 玩家是否在视野内

    // 高亮颜色
    private static readonly Color GoldColor = new Color(1f, 0.84f, 0f);
    private static readonly Color RedColor = new Color(1f, 0.2f, 0.2f);

    private void Start()
    {
        // 获取Renderer
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("PickupItem需要Renderer组件！", this);
            enabled = false;
            return;
        }

        // 初始化MaterialPropertyBlock
        mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        // 检查材质是否支持Emission
        hasEmission = rend.material.HasProperty("_EmissionColor");
        if (!hasEmission)
        {
            Debug.LogWarning("材质不支持Emission，请确保材质启用了Emission！", this);
        }

        // 初始化拾取提示UI
        if (pickupHintUI != null)
        {
            hintText = pickupHintUI.GetComponent<Text>();
            if (hintText != null)
                hintText.fontSize = (int)fontSize;
            pickupHintUI.SetActive(false);
        }

        // 查找玩家（通过Tag或层级）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("场景中未找到Tag为'Player'的游戏对象！", this);
    }

    private void Update()
    {
        // 每帧检测玩家是否在视野内
        UpdatePlayerInView();

        // 如果在视野内且按下拾取键，则拾取
        if (isPlayerInView && Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    /// <summary>
    /// 更新玩家是否在视野内的状态（距离 + 角度检测）
    /// </summary>
    private void UpdatePlayerInView()
    {
        if (playerTransform == null) return;

        // 计算距离
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distance = directionToPlayer.magnitude;

        // 条件1：距离检测
        if (distance > pickupRange)
        {
            // 距离超出范围，取消高亮和提示
            if (isPlayerInView)
            {
                isPlayerInView = false;
                ApplyHighlight(false);
                ShowPickupHint(false);
            }
            return;
        }

        // 获取玩家的前方向量（注意：使用玩家的视角方向）
        Vector3 playerForward = playerTransform.forward;
        // 计算从玩家指向物体的方向
        Vector3 directionToItem = transform.position - playerTransform.position;

        // 计算夹角
        float angle = Vector3.Angle(playerForward, directionToItem);

        // 条件2：角度检测（夹角是否小于视角半角）
        bool isLookingAtItem = angle <= viewAngle / 2f;

        // 更新状态
        if (isLookingAtItem && distance <= pickupRange)
        {
            // 玩家在范围内且面向物体
            if (!isPlayerInView)
            {
                isPlayerInView = true;
                ApplyHighlight(true);
                ShowPickupHint(true);
            }
        }
        else
        {
            // 玩家不在视野内
            if (isPlayerInView)
            {
                isPlayerInView = false;
                ApplyHighlight(false);
                ShowPickupHint(false);
            }
        }
    }

    private void ApplyHighlight(bool highlight)
    {
        if (rend == null || !hasEmission) return;

        rend.GetPropertyBlock(mpb);

        if (highlight)
        {
            Color emissionColor = (highlightType == HighlightType.Gold) ? GoldColor : RedColor;
            mpb.SetColor("_EmissionColor", emissionColor * highlightIntensity);
        }
        else
        {
            mpb.SetColor("_EmissionColor", Color.black);
        }

        rend.SetPropertyBlock(mpb);
    }

    private void ShowPickupHint(bool show)
    {
        if (pickupHintUI != null)
        {
            pickupHintUI.SetActive(show);
            if (hintText != null)
                hintText.fontSize = (int)fontSize;
        }
    }

    private void TryPickup()
    {
        InventoryManager inv = FindFirstObjectByType<InventoryManager>();
        if (inv == null)
        {
            Debug.LogError("场景中未找到InventoryManager！");
            return;
        }

        if (inv.AddItem(itemData))
        {
            ShowPickupHint(false);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("物品栏已满，无法拾取");
        }
    }

    // 可视化调试（在Scene视图中显示检测范围）
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
        {
            // 如果没有玩家引用，尝试用场景中的玩家
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
            else
                return;
        }

        // 绘制距离范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

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

    public enum HighlightType
    {
        Gold,
        Red
    }
}