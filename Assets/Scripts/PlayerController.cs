using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("玩家组件")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform groundCheck;

    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = 20f;

    [Header("视角设置")]
    [SerializeField] private float turnSensitivity = 120f;
    [SerializeField] private float minLookAngle = -70f;
    [SerializeField] private float maxLookAngle = 70f;

    [Header("地面检测")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.25f;

    private Vector3 moveDirection;
    private Vector3 velocity;
    [SerializeField]private bool isGrounded;
    private float xRotation;

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGround();
        Move();
        Jump();
        ApplyGravity();
        TurnView();
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = controller.isGrounded;
            return;
        }

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            checkRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        moveDirection = transform.right * horizontalMove + transform.forward * verticalMove;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -10f;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        if (isGrounded) return;
        
        if (velocity.y is > 0f and < 5f)
        {
            gravity = 80f;
        }
        else if (velocity.y is < 0f and < -5f)
        {
            gravity = 40f;
        }
    }

    private void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void TurnView()
    {
        if (playerCamera == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * turnSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * turnSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
