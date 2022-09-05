using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBridge : MonoBehaviour
{
    Animator anim;
    Camera cam;
    OnGroundBehaviour onGroundBehavoiur;

    private void Awake()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        onGroundBehavoiur = anim.GetBehaviour<OnGroundBehaviour>();
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        onGroundBehavoiur.Forward = cam.transform.forward;
        onGroundBehavoiur.Right = cam.transform.right;

        onGroundBehavoiur.MoveAction = context.ReadValue<Vector2>();
    }
}