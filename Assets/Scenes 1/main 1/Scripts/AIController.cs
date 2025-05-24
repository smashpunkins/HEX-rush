using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5f;
    public float boostedSpeed = 10f;
    public float boostDistanceThreshold = 3f;
    public float rotationSpeed = 10f;
    public float moveThreshold = 0.01f;

    [Header("Push/Bounce Settings")]
    public float bounceForce = 0.8f;
    public float upwardPushFactor = 0.2f;
    public float separationForce = 5f;
    public float platformRadius = 2f;

    [Header("Target Platform")]
    public Transform targetPlatform;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded = true;

    // New variables for AI's post-reach behavior
    private bool isAtTargetPlatform = false;
    private bool canMoveAround = false;
    private float interactionRange = 3f; // Range to check if the AI can interact
    private Vector3 randomPatrolTarget;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (targetPlatform != null)
        {
            if (!isAtTargetPlatform)
            {
                MoveToTarget();
            }
            else if (canMoveAround)
            {
                PatrolPlatform();
                TryAttackPlayerOrAI();
            }
        }

        UpdateAnimations();
    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = new Vector3(targetPlatform.position.x, transform.position.y, targetPlatform.position.z);
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        float currentSpeed = moveSpeed;

        // Boost speed if another AI is nearby
        Collider[] nearbyAIs = Physics.OverlapSphere(transform.position, boostDistanceThreshold);
        foreach (Collider col in nearbyAIs)
        {
            if (col.CompareTag("AI") && col.gameObject != this.gameObject)
            {
                currentSpeed = boostedSpeed;
                break;
            }
        }

        Vector3 moveDirection = direction.normalized * currentSpeed * Time.fixedDeltaTime;

        // Movement logic
        if (direction.magnitude > moveThreshold)
        {
            rb.MovePosition(transform.position + moveDirection);

            // Smoothly rotate toward target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            {
                animator.SetTrigger("StartRunning");
            }
        }
        else
        {
            if (!isAtTargetPlatform)
            {
                isAtTargetPlatform = true;
                canMoveAround = true;
                SetRandomPatrolTarget();
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            {
                animator.SetTrigger("StartIdle");
            }
        }

        // Check for obstacles (AI and player) on the platform
        CheckAndPushOff();
    }

    private void CheckAndPushOff()
    {
        float platformRadius = targetPlatform.GetComponent<Collider>().bounds.extents.magnitude;

        Collider[] nearbyEntities = Physics.OverlapSphere(targetPlatform.position, platformRadius);
        bool isPlatformOccupied = false;

        foreach (Collider col in nearbyEntities)
        {
            if (col.CompareTag("AI") && col.gameObject != this.gameObject)
            {
                isPlatformOccupied = true;
                Vector3 separationDirection = transform.position - col.transform.position;
                separationDirection.y = 0f;

                float distance = separationDirection.magnitude;

                // Apply force to push away if too close
                if (distance < platformRadius * 0.7f)  // Adjust this threshold to prevent crowding
                {
                    float pushForce = separationForce * (1f - distance / (platformRadius * 0.7f)); // Increase force if closer
                    col.attachedRigidbody.AddForce(separationDirection.normalized * pushForce, ForceMode.Impulse);
                }
            }

            if (col.CompareTag("Player"))
            {
                isPlatformOccupied = true;
                Vector3 separationDirection = transform.position - col.transform.position;
                separationDirection.y = 0f;

                float distance = separationDirection.magnitude;

                // Apply force to push player off the platform if too close
                if (distance < platformRadius * 0.7f)
                {
                    float pushForce = separationForce * (1f - distance / (platformRadius * 0.7f));
                    col.attachedRigidbody.AddForce(separationDirection.normalized * pushForce, ForceMode.Impulse);
                }
            }
        }

        // If platform is occupied and the AI is stuck, apply a random force to avoid staying stuck
        if (isPlatformOccupied)
        {
            Vector3 randomForce = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * separationForce * 0.3f;
            rb.AddForce(randomForce, ForceMode.Impulse);
        }
    }

    private void SetRandomPatrolTarget()
    {
        // Set a new random position on the platform for the AI to patrol
        randomPatrolTarget = new Vector3(
            targetPlatform.position.x + Random.Range(-platformRadius, platformRadius),
            targetPlatform.position.y,
            targetPlatform.position.z + Random.Range(-platformRadius, platformRadius)
        );
    }

    private void PatrolPlatform()
    {
        // AI moves toward the random target position on the platform
        Vector3 direction = randomPatrolTarget - transform.position;
        direction.y = 0f;

        if (direction.magnitude > moveThreshold)
        {
            rb.MovePosition(transform.position + direction.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // When the patrol target is reached, set a new random target
            SetRandomPatrolTarget();
        }

        // Smooth rotation while patrolling
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void TryAttackPlayerOrAI()
    {
        // Check if the player or another AI is within range to attack
        Collider[] nearbyEntities = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (Collider col in nearbyEntities)
        {
            if (col.CompareTag("Player") || col.CompareTag("AI"))
            {
                // Implement attack logic here
                Debug.Log($"Attacking {col.name}");
            }
        }
    }

    private void UpdateAnimations()
    {
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (horizontalSpeed > moveThreshold)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            {
                animator.SetTrigger("StartRunning");
            }
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            {
                animator.SetTrigger("StartIdle");
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("AI"))
        {
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb != null)
            {
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                pushDir.y = upwardPushFactor;

                // Apply force to push the other AI without recoil to the original AI
                otherRb.AddForce(pushDir * bounceForce, ForceMode.Impulse);
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // Set isGrounded to true when touching the ground
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Set isGrounded to false when leaving the ground
        }
    }

    public void SetNewTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            targetPlatform = newTarget;
            Debug.Log($"New target platform set: {newTarget.name}");
        }
    }
}
