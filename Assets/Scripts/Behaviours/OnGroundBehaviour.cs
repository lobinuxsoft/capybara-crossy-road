using UnityEngine;

public class OnGroundBehaviour : StateMachineBehaviour
{
    int velocityHash = Animator.StringToHash("Velocity");
    int groundHash = Animator.StringToHash("OnGround");

    public Vector2 MoveAction { get; set; }
    public bool JumpAction { get; set; }
    [SerializeField] public bool xAction { get; set; }

    [SerializeField, Range(1, 20)] float maxAcceleration = 10f, maxSpeed = 12f, rotationSpeed = 2f;
    [SerializeField, Range(1, 10)] float jumpHeight = 2;

    Rigidbody rb = default;
    GroundDetector groundDetector = default;
    Vector2 movementVector = Vector2.zero;
    Vector3 lookDirection = Vector3.zero;
    Vector3 movement = Vector3.zero, velocity = Vector3.zero;
    Vector3 forward = Vector3.forward, right = Vector3.right;

    public Vector3 Forward
    {
        get => forward;
        set => forward = value;
    }

    public Vector3 Right
    {
        get => right;
        set => right = value;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundDetector = animator.GetComponent<GroundDetector>();
        rb = animator.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MoveUpdate();

        animator.SetFloat(velocityHash, rb.velocity.magnitude / maxSpeed);
        animator.SetBool(groundHash, groundDetector.OnGround);
    }

    private void MoveDirection(Vector2 value)
    {
        movementVector = value;

        forward.y = 0;
        right.y = 0;

        lookDirection = movementVector.magnitude > 0 ? forward * movementVector.y + right * movementVector.x : lookDirection;

        movement = movementVector.magnitude > 0 ? lookDirection * maxSpeed : Vector3.zero;

        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(lookDirection, rb.transform.up), Time.deltaTime * rotationSpeed);
    }

    private void JumpLogic(bool jump)
    {
        if (jump)
        {
            float jumpSpeed = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, groundDetector.Normal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }

            velocity += groundDetector.Normal * jumpSpeed;
        }
    }

    private void MoveUpdate()
    {
        velocity = rb.velocity;
        float maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;

        MoveDirection(MoveAction);
        JumpLogic(groundDetector.OnGround && JumpAction);

        velocity.x = Mathf.MoveTowards(velocity.x, movement.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, movement.z, maxSpeedChange);

        rb.velocity = velocity;
    }
}