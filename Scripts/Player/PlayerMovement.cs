using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public Transform cameraT;
    public Rigidbody rb;

    public SFXBank footstepsBank;
    public float fixedSFXDelay = 0.7f;
    public float sfxDelay;
    private float prevFootstepTime;

    public float fixedSpeed = 5f;
    public float sprintSpeed = 10f;
    public float speedMult = 1f; 

    public float jumpForce = 5f;
    public float jumpCD = 0.5f;

    public bool isGrounded = true;

    private float speed;
    private float prevJumpTime;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        if(rb.velocity != Vector3.zero && Time.time > sfxDelay + prevFootstepTime && isGrounded)
        {
            prevFootstepTime = Time.time;
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            SFXManager.Instance.Play(footstepsBank, pos,0.2f);
        }
    }
    private void FixedUpdate()
    {
        Movement();
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.LeftShift) && StaminaManager.Instance.CheckStamina())
        {
            speed = sprintSpeed;
            if (speedMult > 1.05f) sfxDelay = fixedSFXDelay / 3f;
            else sfxDelay = fixedSFXDelay/2;
            StaminaManager.Instance.DrainStamina();
        }
        else
        {
            speed = fixedSpeed;
            if (speedMult > 1.05f) sfxDelay = fixedSFXDelay / 1.7f;
            else sfxDelay = fixedSFXDelay;
        }
    }
    private void Movement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = cameraT.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraT.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDirection = forward * moveZ + right * moveX;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        float finalSpeed = speed * speedMult;
        rb.velocity = new Vector3(
            moveDirection.x * finalSpeed,
            rb.velocity.y,
            moveDirection.z * finalSpeed
        );
    }
    private void Jump()
    {
        if (prevJumpTime + jumpCD <= Time.time && isGrounded)
        {
            StaminaManager.Instance.DrainStamina(10);
            prevJumpTime = Time.time;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    public void SetSpeedMult(float value, float dur)
    {
        DOTween.Kill("SpeedMultTween");
        Sequence seq = DOTween.Sequence().SetId("SpeedMultTween");
        seq.AppendCallback(() =>
        {
            FOVManager.Instance.IsSpeedMult = true;
        });
        seq.Append(DOTween.To(() => speedMult, x => speedMult = x, value, 0.25f));
        seq.AppendInterval(dur);
        seq.AppendCallback(() =>
        {
            FOVManager.Instance.IsSpeedMult = false;
        });
        seq.Append(DOTween.To(() => speedMult, x => speedMult = x, 1f, 2f));
    }
    public void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("Ground") && prevJumpTime + jumpCD <= Time.time)
        {
            isGrounded = true;
        }
    }
}
