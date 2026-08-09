using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject mainCamera;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 防止重复
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 跨场景不销毁
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnGameStart()
    {
        mainCamera.SetActive(false);
        player.SetActive(true);
    }
}
