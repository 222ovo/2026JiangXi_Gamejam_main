using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("物品数据")]
    public Item itemData;

    [Header("高亮类型")]
    public HighlightType highlightType = HighlightType.Gold; // 在Inspector中选择

    [Header("高亮强度")]
    [Range(0.1f, 3f)]
    public float highlightIntensity = 1.5f; // 发光强度

    [Header("拾取设置")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupRange = 2f;

    // 内部变量
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private bool isPlayerInRange = false;
    private bool hasEmission;

    // 定义两种高亮颜色
    private static readonly Color GoldColor = new Color(1f, 0.84f, 0f);      // 金色
    private static readonly Color RedColor = new Color(1f, 0.2f, 0.2f);      // 红色

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("PickupItem需要Renderer组件", this);
            enabled = false;
            return;
        }

        mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        // 检查材质是否支持Emission
        hasEmission = mpb.HasColor("_EmissionColor") || rend.material.HasProperty("_EmissionColor");
        if (!hasEmission)
        {
            Debug.LogWarning("材质不支持Emission，高亮将无法显示。请确保材质启用了Emission关键词。", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ApplyHighlight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ApplyHighlight(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    private void ApplyHighlight(bool highlight)
    {
        if (rend == null || !hasEmission) return;

        rend.GetPropertyBlock(mpb);

        if (highlight)
        {
            // 根据高亮类型设置Emission颜色
            Color emissionColor = (highlightType == HighlightType.Gold) ? GoldColor : RedColor;
            mpb.SetColor("_EmissionColor", emissionColor * highlightIntensity);
        }
        else
        {
            // 关闭Emission
            mpb.SetColor("_EmissionColor", Color.black);
        }

        rend.SetPropertyBlock(mpb);
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
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("物品栏已满，无法拾取");
        }
    }

    public enum HighlightType
    {
        Gold,   // 金色高亮
        Red     // 红色高亮
    }
}
