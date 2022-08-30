#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor;
#endif

using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Vector3 applyForce = new Vector3(0, 4f, 1.25f);

    Rigidbody rb;
    Vector3 viewDir = Vector3.forward;

    public static Action<int> OnJump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

#if UNITY_EDITOR
        CreateSceneParameters sceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        scenePrediction = SceneManager.CreateScene($"Simulation {gameObject.GetHashCode()}", sceneParameters);
        scenePredictionPhysics = scenePrediction.GetPhysicsScene();
#endif
    }

    public void JumpToDirection(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.sqrMagnitude > 0)
        {
            viewDir = new Vector3(input.x, 0, input.y);

            transform.rotation = Quaternion.LookRotation(viewDir, transform.up);

#if UNITY_EDITOR
            JumpSimulation(transform, applyForce);
#endif

            rb.AddForce((transform.forward * applyForce.z + transform.up * applyForce.y) * 10, ForceMode.Impulse);

            OnJump((int)rb.position.z);
        }
        else
        {
            viewDir = new Vector3(0, 0, 1);

            transform.rotation = Quaternion.LookRotation(viewDir, transform.up);

#if UNITY_EDITOR
            JumpSimulation(transform, applyForce);
#endif

            rb.AddForce((transform.forward * applyForce.z + transform.up * applyForce.y) * 10, ForceMode.Impulse);

            OnJump((int)rb.position.z);
        }
    }

#if UNITY_EDITOR

    private Scene scenePrediction;
    private PhysicsScene scenePredictionPhysics;

    [Header("Physics simulation (Editor Only)")]
    [SerializeField] int maxSteps = 40;
    [SerializeField] Color gizmosColor = Color.yellow;
    List<Vector3> positions = new List<Vector3>();

    private void JumpSimulation(in Transform objRef, Vector3 force)
    {
        if (!scenePredictionPhysics.IsValid())
            return;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = objRef.position;
        cube.transform.rotation = objRef.rotation;
        cube.transform.localScale = objRef.localScale;

        SceneManager.MoveGameObjectToScene(cube, scenePrediction);
        cube.AddComponent<Rigidbody>().AddForce(cube.transform.forward * force.z + cube.transform.up * force.y, ForceMode.Impulse);

        positions.Clear();

        for (int i = 0; i < maxSteps; i++)
        {
            scenePredictionPhysics.Simulate(Time.fixedDeltaTime);

            positions.Add(cube.transform.position);
        }

        Destroy(cube);
    }

    private void OnDrawGizmos()
    {
        Handles.ArrowHandleCap(
                                0,
                                transform.position,
                                Quaternion.LookRotation(transform.forward * applyForce.z + transform.up * applyForce.y, transform.up),
                                1,
                                EventType.Repaint
                              );

        if (positions.Count > 0)
        {
            Handles.color = gizmosColor;

            foreach (var pos in positions)
            {
                Handles.SphereHandleCap(0, pos, Quaternion.identity, .05f, EventType.Repaint);
            }
        }
    }
#endif
}