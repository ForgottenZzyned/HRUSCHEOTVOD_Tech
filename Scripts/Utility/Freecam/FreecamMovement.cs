using UnityEditor.Rendering;
using UnityEngine;

public class FreecamMovement : MonoBehaviour
{
    public Transform cameraT;
    public float speed = 1.0f;
    [SerializeField] private float speedStep = 2f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 30f;
    private Vector3 currentVelocity;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 12f;
    [Header("PovMovement")]
    private float xRot;
    private float yRot;
    private float smoothMouseX;
    private float smoothMouseY;
    private bool isVectorAccurate = false;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float mouseSmooth = 10f;
    private void Start()
    {
        CursorManager.SetCursor(false);
    }
    public void Update()
    {
        FreeMovement();
        PovMovement();
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            speed = Mathf.Clamp(
                speed + scroll * speedStep,
                minSpeed,
                maxSpeed
            );
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            isVectorAccurate = !isVectorAccurate;
        }
    }
    private void PovMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        smoothMouseX = Mathf.Lerp(
            smoothMouseX,
            mouseX,
            mouseSmooth * Time.deltaTime
        );
        smoothMouseY = Mathf.Lerp(
            smoothMouseY,
            mouseY,
            mouseSmooth * Time.deltaTime
        );

        yRot += smoothMouseX;
        xRot -= smoothMouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(
            xRot,
            yRot,
            0f
        );
    }
    private void FreeMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;
        if (Input.GetKey(KeyCode.E))
            moveY = 1f;
        else if (Input.GetKey(KeyCode.Q))
            moveY = -1f;
        Vector3 forward;
        Vector3 right;
        if (isVectorAccurate) forward = Vector3.forward;
        else forward = cameraT.forward;
        //forward.y = 0f;
        forward.Normalize();

        if (isVectorAccurate) right = Vector3.right;
        else right = cameraT.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDirection =
            forward * moveZ +
            right * moveX +
            Vector3.up * moveY;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        float finalSpeed = speed;
        Vector3 targetVelocity = moveDirection * finalSpeed;

        float smoothSpeed = moveDirection.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;
        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            smoothSpeed * Time.deltaTime
        );
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
    }
}
