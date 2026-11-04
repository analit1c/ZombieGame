using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;

    [Header("Jump")]
    public float jumpForce = 6f;

    [Header("Mouse Camera")]
    public float mouseSensitivity = 2f;
    public float cameraPitch = 50f;
    public Vector3 cameraLocalPosition = new Vector3(0f, 10f, -8f);

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Transform cameraTransform;

    private float horizontalInput;
    private float verticalInput;
    private float currentYaw;

    private bool jumpPressed;
    private bool cursorLocked = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;

            // Камера должна быть дочерней для Player
            cameraTransform.localPosition = cameraLocalPosition;
            cameraTransform.localRotation =
                Quaternion.Euler(cameraPitch, 0f, 0f);
        }
        else
        {
            Debug.LogError("Main Camera не найдена!");
        }

        currentYaw = transform.eulerAngles.y;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        LockCursor();
    }

    void Update()
    {
        // Управление клавиатурой
        horizontalInput = 0f;
        verticalInput = 0f;

        if (Input.GetKey(KeyCode.W))
            verticalInput = 1f;

        if (Input.GetKey(KeyCode.S))
            verticalInput = -1f;

        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;

        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;

        // Управление курсором
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0) && !cursorLocked)
        {
            LockCursor();
        }

        // Поворот камеры мышью
        if (cursorLocked)
        {
            float mouseX = Input.GetAxis("Mouse X");

            currentYaw += mouseX * mouseSensitivity;
        }
    }

    void FixedUpdate()
    {
        // Поворачиваем игрока
        Quaternion targetRotation =
            Quaternion.Euler(0f, currentYaw, 0f);

        rb.MoveRotation(targetRotation);

        MovePlayer();

        if (jumpPressed)
        {
            if (IsGrounded())
            {
                rb.AddForce(
                    Vector3.up * jumpForce,
                    ForceMode.Impulse
                );
            }

            jumpPressed = false;
        }
    }

    void MovePlayer()
    {
        // Направление движения относительно игрока
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 movement =
            forward * verticalInput +
            right * horizontalInput;

        movement = Vector3.ClampMagnitude(movement, 1f);

        float speed = Input.GetKey(KeyCode.LeftShift)
            ? moveSpeed * sprintMultiplier
            : moveSpeed;

        Vector3 velocity = rb.linearVelocity;

        rb.linearVelocity = new Vector3(
            movement.x * speed,
            velocity.y,
            movement.z * speed
        );
    }

    bool IsGrounded()
    {
        Vector3 origin = new Vector3(
            capsule.bounds.center.x,
            capsule.bounds.min.y + 0.05f,
            capsule.bounds.center.z
        );

        return Physics.Raycast(
            origin,
            Vector3.down,
            0.2f
        );
    }

    void LockCursor()
    {
        cursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
