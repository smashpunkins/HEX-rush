using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;            // Object to follow
    public float distance = 5.0f;       // Distance behind the target
    public float height = 2.0f;         // Height above the target
    public float rotationSpeed = 100f;  // Mouse rotation sensitivity
    public float followDamping = 0.1f;  // Camera movement damping
    public float lookDamping = 0.1f;    // Look-at smoothing

    private float currentYaw = 0f;
    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (!target) return;

        // Get mouse horizontal input
        currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

        // Calculate desired camera rotation
        Quaternion rotation = Quaternion.Euler(0, currentYaw, 0);

        // Calculate desired camera position behind and above the target
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + Vector3.up * height + offset;

        // Smoothly move the camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followDamping);

        // Smoothly look at the target
        Quaternion lookRotation = Quaternion.LookRotation(target.position + Vector3.up * height - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookDamping);
    }
}
