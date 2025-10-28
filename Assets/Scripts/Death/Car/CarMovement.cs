using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

[RequireComponent(typeof(NavMeshAgent))]
public class CarMovement : MonoBehaviour
{
    [Header("Spline Settings")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField, Range(0, 1)] private float splineProgress = 0f;
    [SerializeField] private float progressSpeed = 0.05f;

    [Header("Movement Settings")]
    [SerializeField] private float lookAhead = 0.02f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float navMeshSampleRadius = 3f;

    private NavMeshAgent agent;
    private bool isMoving;
    private float splineLength;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    private void Start()
    {
        if (splineContainer == null)
        {
            Debug.LogWarning($"{name}: Aucun SplineContainer assigné.");
            enabled = false;
            return;
        }

        splineLength = SplineUtility.CalculateLength(splineContainer.Spline, splineContainer.transform.localToWorldMatrix);

        Vector3 startPos = splineContainer.transform.TransformPoint(splineContainer.Spline.EvaluatePosition(0f));
        if (NavMesh.SamplePosition(startPos, out var hit, navMeshSampleRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }   
        else
        {
            Debug.LogWarning($"{name}: impossible de placer l'agent sur le NavMesh au début de la spline.");
        }
            
        if (autoStart) StartMovement();
    }

    private void Update()
    {
        if (!isMoving || splineContainer == null) return;

        splineProgress += (agent.speed * Time.deltaTime) / splineLength;
        if (splineProgress > 1f)
        {
            splineProgress -= 1f;
        }

        Vector3 targetPos = splineContainer.transform.TransformPoint(
            splineContainer.Spline.EvaluatePosition(splineProgress)
        );

        if (NavMesh.SamplePosition(targetPos, out var hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        Vector3 lookPos = splineContainer.Spline.EvaluatePosition(Mathf.Clamp01(splineProgress + lookAhead));
        lookPos = splineContainer.transform.TransformPoint(lookPos);
        Vector3 dir = (lookPos - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * agent.angularSpeed / 10f);
        }
    }

    public void StartMovement() => isMoving = true;
    public void StopMovement() => isMoving = false;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (splineContainer == null) return;

        Gizmos.color = Color.cyan;
        var spline = splineContainer.Spline;
        int steps = 50;
        Vector3 prev = splineContainer.transform.TransformPoint(spline.EvaluatePosition(0f));

        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector3 pos = splineContainer.transform.TransformPoint(spline.EvaluatePosition(t));
            Gizmos.DrawLine(prev, pos);
            prev = pos;
        }

        Gizmos.color = Color.red;
        Vector3 current = splineContainer.transform.TransformPoint(splineContainer.Spline.EvaluatePosition(splineProgress));
        Gizmos.DrawSphere(current, 0.15f);
    }
#endif
}
