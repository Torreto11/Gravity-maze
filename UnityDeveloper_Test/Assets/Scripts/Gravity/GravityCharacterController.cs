using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 20f;
    public float groundedDrag = 8f;
    public float airControlPercent = 0.35f;

    [Header("Gravity")]
    public float gravityStrength = 25f;
    public float airborneAlignSpeed = 10f;
    public float groundedAlignSpeed = 25f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.7f;
    public float groundCheckRadius = 0.3f;
    public LayerMask walkableMask;

    [Header("References")]
    public Transform visualModel;
    public Transform cameraTransform;
    public Animator animator;

    [Header("State")]
    public Vector3 targetGravityUp = Vector3.up;
    public bool isGrounded;

    private Rigidbody rb;
    private Vector2 moveInput;
    private RaycastHit groundHit;
    private Vector3 stableGroundUp = Vector3.up;

    public Vector3 GravityDown => -targetGravityUp;
    public Vector3 CurrentUp => isGrounded ? stableGroundUp : targetGravityUp;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyCustomGravity();
        AlignBody();
        HandleMovement();
        ApplyDrag();
        RotateVisualModel();
    }

    public void RotateGravity(Vector3 newUp)
    {
        targetGravityUp = SnapToAxis(newUp);
    }

    public void ClearVelocityAlongGravity()
    {
        Vector3 lateral = Vector3.ProjectOnPlane(rb.velocity, targetGravityUp);
        rb.velocity = lateral;
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            stableGroundUp = targetGravityUp;
            return;
        }

        isGrounded = Physics.SphereCast(
            groundCheck.position,
            groundCheckRadius,
            GravityDown,
            out groundHit,
            groundCheckDistance,
            walkableMask,
            QueryTriggerInteraction.Ignore
        );

        if (isGrounded)
        {
            stableGroundUp = SnapToAxis(groundHit.normal);
        }
        else
        {
            stableGroundUp = targetGravityUp;
        }
    }

    private void ApplyCustomGravity()
    {
        rb.AddForce(GravityDown * gravityStrength, ForceMode.Acceleration);
    }

    private void AlignBody()
    {
        Vector3 desiredUp = isGrounded ? stableGroundUp : targetGravityUp;
        float alignSpeed = isGrounded ? groundedAlignSpeed : airborneAlignSpeed;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, desiredUp) * rb.rotation;
        Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, alignSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothRotation);

        // When grounded, force a very exact alignment so there is no slant.
        if (isGrounded && Vector3.Angle(transform.up, stableGroundUp) < 2f)
        {
            Quaternion exact = Quaternion.FromToRotation(transform.up, stableGroundUp) * transform.rotation;
            rb.MoveRotation(exact);
        }
    }

    private void HandleMovement()
    {
        if (cameraTransform == null) return;

        Vector3 movementUp = CurrentUp;

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, movementUp).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, movementUp).normalized;

        if (camForward.sqrMagnitude < 0.0001f)
            camForward = Vector3.ProjectOnPlane(transform.forward, movementUp).normalized;

        if (camRight.sqrMagnitude < 0.0001f)
            camRight = Vector3.Cross(movementUp, camForward).normalized;

        Vector3 moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        Vector3 currentLateralVelocity = Vector3.ProjectOnPlane(rb.velocity, movementUp);

        float control = isGrounded ? 1f : airControlPercent;
        Vector3 velocityDelta = (targetVelocity - currentLateralVelocity) * (acceleration * control * Time.fixedDeltaTime);

        rb.AddForce(velocityDelta, ForceMode.VelocityChange);
    }

    private void ApplyDrag()
    {
        if (!isGrounded) return;

        Vector3 up = CurrentUp;
        Vector3 lateral = Vector3.ProjectOnPlane(rb.velocity, up);
        Vector3 vertical = Vector3.Project(rb.velocity, up);

        lateral *= 1f / (1f + groundedDrag * Time.fixedDeltaTime);
        rb.velocity = lateral + vertical;
    }

    private void RotateVisualModel()
    {
        if (visualModel == null) return;

        Vector3 up = CurrentUp;
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(rb.velocity, up);

        if (lateralVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lateralVelocity.normalized, up);
            visualModel.rotation = Quaternion.Slerp(
                visualModel.rotation,
                targetRotation,
                12f * Time.fixedDeltaTime
            );
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        Vector3 up = CurrentUp;
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(rb.velocity, up);
        float speed = lateralVelocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private Vector3 SnapToAxis(Vector3 v)
    {
        v.Normalize();

        float ax = Mathf.Abs(v.x);
        float ay = Mathf.Abs(v.y);
        float az = Mathf.Abs(v.z);

        if (ax >= ay && ax >= az)
            return new Vector3(Mathf.Sign(v.x), 0f, 0f);

        if (ay >= ax && ay >= az)
            return new Vector3(0f, Mathf.Sign(v.y), 0f);

        return new Vector3(0f, 0f, Mathf.Sign(v.z));
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + GravityDown * groundCheckDistance);
    }
}