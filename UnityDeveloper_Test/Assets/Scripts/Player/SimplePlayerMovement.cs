using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float acceleration = 20f;
    public float groundDrag = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("References")]
    public Transform cameraTransform;
    public Transform visualModel;
    public Animator animator;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        CheckGround();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyDrag();
    }

    private void HandleMovement()
    {
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        Vector3 currentVelocity = rb.velocity;
        Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 velocityChange = targetVelocity - currentHorizontalVelocity;
        velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (visualModel != null && moveDirection.sqrMagnitude > 0.001f && isGrounded)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            visualModel.rotation = Quaternion.Slerp(
                visualModel.rotation,
                targetRotation,
                12f * Time.fixedDeltaTime
            );
        }
    }

    private void ApplyDrag()
    {
        if (!isGrounded) return;

        Vector3 velocity = rb.velocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        horizontal *= 1f / (1f + groundDrag * Time.fixedDeltaTime);

        rb.velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float speed = horizontalVelocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}