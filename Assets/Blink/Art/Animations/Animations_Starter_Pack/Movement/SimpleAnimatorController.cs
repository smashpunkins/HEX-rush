using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private bool isJumping = false;
    private bool isGrounded = true;

    public float movementThreshold = 0.1f; // Minimum speed to trigger idle
    public float minSpeedToRun = 2f; // Minimum speed to trigger running animation
    public float moveSpeed = 5f; // Character movement speed

    void Start()
    {
        // Get the Animator and Rigidbody components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Ensure the character is moving at the start
        MoveCharacter(Vector3.forward);  // Move forward immediately upon start
    }

    void Update()
    {
        // Get the horizontal movement speed (magnitude of velocity in X and Z directions)
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        // Handle running animation: check if horizontal speed is greater than the threshold
        if (horizontalSpeed > minSpeedToRun)
        {
            if (!animator.GetBool("isRunning")) // Start running if not already running
            {
                animator.SetBool("isRunning", true);
            }
        }
        else
        {
            if (animator.GetBool("isRunning")) // Stop running if the character is no longer moving fast enough
            {
                animator.SetBool("isRunning", false);
            }
        }

        // Handle idle animation trigger (when the character is not moving)
        if (horizontalSpeed < movementThreshold && isGrounded)
        {
            animator.SetTrigger("IdleTrigger"); // Trigger idle animation
        }

        // Handle jump animation: check if the character is in the air
        if (!isGrounded && !isJumping)  // Character just left the ground
        {
            isJumping = true;
            animator.SetBool("isJumping", true); // Trigger jump animation
        }
        else if (isGrounded && isJumping)  // Character just landed
        {
            isJumping = false;
            animator.SetBool("isJumping", false); // Stop jump animation when landing
        }
    }

    // Detect when the character touches the ground
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // The character is touching the ground
        }
    }

    // Detect when the character leaves the ground
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // The character is no longer touching the ground
        }
    }

    // This method allows you to move the character by applying force or velocity to the Rigidbody
    public void MoveCharacter(Vector3 direction)
    {
        // Apply movement immediately on start to avoid the delay issue
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
    }
}
