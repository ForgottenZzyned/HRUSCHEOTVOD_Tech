using UnityEngine;
using Cinemachine;
public class CameraMovement : Singleton<CameraMovement>
{
    public Transform playerT;
    public float mouseSensitivity = 100f;
    public float mouseSmooth = 10f;
    private CinemachineVirtualCamera vcam;
    private float xRot = 0f;
    private float smoothMouseX;
    private float smoothMouseY;
    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        FOVManager.Instance.SerializeCamera(this);
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
    }
    private void Update()
    {
        PovMovement();
    }
    private void PovMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
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
        xRot -= smoothMouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerT.Rotate(Vector3.up * smoothMouseX);
    }
    public CinemachineVirtualCamera GetCamera()
    {
        if(vcam != null)return vcam;
        return null;
    }
}
