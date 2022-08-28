#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float jumpHeigh = 2;

    Rigidbody rb;
    Vector3 viewDir = Vector3.forward;

    public static Action<int> OnJump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void JumpToDirection(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input.magnitude > 0)
        {
            viewDir = new Vector3(input.x, 0, input.y);

            transform.rotation = Quaternion.LookRotation(viewDir, transform.up);

            rb.velocity = CalculateJumpData(viewDir, jumpHeigh, Physics.gravity.y).initialVelocity;

            OnJump((int)rb.transform.position.z);
        }
    }

    private JumpData CalculateJumpData(Vector3 direction, float heigh,float gravity)
    {
        Vector3 targetPos = transform.position + direction;

        float displacementY = targetPos.y - transform.position.y;
        Vector3 displacementXZ = new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z);

        float time = Mathf.Sqrt(-2 * heigh / gravity) + Mathf.Sqrt(2 * (displacementY - heigh) / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * heigh);
        Vector3 velocityXZ = displacementXZ / time;

        return new JumpData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    struct JumpData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public JumpData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (rb != null && rb.IsSleeping())
        {
            DrawPath();
        }
    }

    private void DrawPath()
    {
        JumpData jumpData = CalculateJumpData(viewDir, jumpHeigh, Physics.gravity.y);

        Vector3 previousDrawPoint = transform.position;
        Vector3 targetPos = transform.position + viewDir;

        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, .5f);

        Handles.color = Color.green;
        Handles.DrawWireDisc(targetPos, Vector3.up, .5f);

        int resolution = 30;
        for (int i = 0; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * jumpData.timeToTarget;
            Vector3 displacement = jumpData.initialVelocity * simulationTime + Physics.gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            Handles.color = Color.yellow;
            Handles.DrawLine(previousDrawPoint, drawPoint);
            previousDrawPoint = drawPoint;
        }
    }
#endif
}