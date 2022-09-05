#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class RigidbodyMoveDebuger : MonoBehaviour
{

#if UNITY_EDITOR

    [SerializeField] float height = .5f;
    [SerializeField] Color forwardColor = Color.blue;
    [SerializeField] Color dirColor = Color.red;
    [SerializeField] Color angleColor = new Color(Color.red.r, Color.red.g, Color.red.b, .5f);
    float averangedSpeed = 0;
    Rigidbody rb = default;

    private void OnDrawGizmos()
    {
        if(!rb) rb = GetComponent<Rigidbody>();

        if(rb.velocity.magnitude > averangedSpeed) averangedSpeed = rb.velocity.magnitude;

        Vector3 center = rb.position + (rb.transform.up * height);
        Vector3 normal = rb.transform.up;
        Vector3 forward = rb.transform.forward;

        Handles.color = angleColor;
        Handles.DrawSolidArc(center, normal, forward, Vector3.SignedAngle(forward, rb.velocity, normal), rb.velocity.magnitude / averangedSpeed);

        Handles.color = forwardColor;
        Handles.DrawWireDisc(center, normal, rb.transform.forward.magnitude);

        Handles.color = forwardColor;
        Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(forward, normal), forward.magnitude, EventType.Repaint);

        Handles.color = dirColor;
        Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(rb.velocity, normal), rb.velocity.magnitude / averangedSpeed, EventType.Repaint);
    }

#endif

}
