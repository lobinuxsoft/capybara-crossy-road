using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float jumpForce = 10;

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
            Vector3 jumpDir = viewDir + transform.up;

            transform.rotation = Quaternion.LookRotation(viewDir, transform.up);

            rb.AddForce(jumpDir * jumpForce);
            OnJump((int)rb.transform.position.z);
        }
    }
}