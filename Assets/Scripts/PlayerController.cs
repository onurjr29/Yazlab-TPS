using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float sprintSpeed = 6.5f;
    public float acceleration = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.1f;

    [Header("Camera")]
    public Transform cameraRoot;   // CameraRoot (player child)
    public Transform cam;          // Actual Camera (child of CameraRoot)
    public float mouseSensitivity = 0.01f;// deg/sec
    public float minPitch = -40f;
    public float maxPitch = 70f;

    private CharacterController controller;
    private float pitch;
    private Vector3 velocity;
    private float currentSpeed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraRoot == null || cam == null)
            Debug.LogWarning("Assign cameraRoot and cam on PlayerController!");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        // Yatay dönme (player gövdesi)
        transform.Rotate(Vector3.up, mouseX * mouseSensitivity);

        // Dikey dönme (kamera pitch)
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }


    void HandleMove()
    {
        // Kamera yönüne göre hareket eksenleri
        Vector3 forward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
        Vector3 right = new Vector3(cam.right.x, 0f, cam.right.z).normalized;

        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S
        Vector3 target = (forward * v + right * h).normalized;

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, target.magnitude * targetSpeed, acceleration * Time.deltaTime);

        // Yatay hareket
        Vector3 move = target * currentSpeed;

        // Yer kontrolü
        if (controller.isGrounded)
        {
            velocity.y = -2f; // yere yapıştır
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move((move + velocity) * Time.deltaTime);
    }
}