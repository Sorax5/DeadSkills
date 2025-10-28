using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class CarDeath : MonoBehaviour
{
    [SerializeField] private SplineContainer SplineContainer;
    private NavMeshAgent navMeshAgent;

    // sampling data
    private Vector3[] samplePositions;
    private float[] sampleDistances;
    private float splineLength;
    private const int SampleCount = 200;

    // runtime tracking for gizmos
    private float currentDistanceAlong = 0f;

    // smoothing
    public float positionLerpSpeed = 8f;
    public float rotationLerpSpeed = 10f;
    public float lookAheadDistance = 1f;

    private void Awake()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
    }

    private void Start()
    {
        FollowSpline();
    }

    public void FollowSpline()
    {
        if (SplineContainer == null || navMeshAgent == null)
        {
            Debug.LogWarning("SplineContainer or NavMeshAgent is not assigned.");
            return;
        }

        Spline spline = SplineContainer.Spline;
        BuildSamples(spline, SampleCount);
        StartCoroutine(FollowSplineCoroutine());
    }

    private void BuildSamples(Spline spline, int samples)
    {
        samplePositions = new Vector3[samples];
        sampleDistances = new float[samples];

        Transform containerT = SplineContainer != null ? SplineContainer.transform : null;

        Vector3 prevLocal = spline.EvaluatePosition(0f);
        Vector3 prevWorld = containerT != null ? containerT.TransformPoint(prevLocal) : prevLocal;
        samplePositions[0] = prevWorld;
        sampleDistances[0] = 0f;
        float acc = 0f;

        for (int i = 1; i < samples; i++)
        {
            float t = (float)i / (samples - 1);
            Vector3 posLocal = spline.EvaluatePosition(t);
            Vector3 posWorld = containerT != null ? containerT.TransformPoint(posLocal) : posLocal;
            acc += Vector3.Distance(prevWorld, posWorld);
            samplePositions[i] = posWorld;
            sampleDistances[i] = acc;
            prevWorld = posWorld;
        }

        splineLength = acc;
    }

    private Vector3 EvaluatePositionAtDistance(float distance)
    {
        if (samplePositions == null || samplePositions.Length == 0)
            return transform.position;

        if (splineLength <= 0f)
            return samplePositions[0];

        distance = Mathf.Repeat(distance, splineLength);

        int idx = 0;
        while (idx < sampleDistances.Length - 1 && sampleDistances[idx + 1] < distance)
            idx++;

        int next = Mathf.Min(idx + 1, sampleDistances.Length - 1);
        float segStart = sampleDistances[idx];
        float segEnd = sampleDistances[next];
        if (segEnd - segStart <= Mathf.Epsilon)
            return samplePositions[idx];

        float t = (distance - segStart) / (segEnd - segStart);
        return Vector3.Lerp(samplePositions[idx], samplePositions[next], t);
    }

    private float FindClosestDistanceOnSpline(Vector3 worldPos)
    {
        if (samplePositions == null || samplePositions.Length == 0)
            return 0f;

        float bestDist = float.MaxValue;
        int bestIdx = 0;
        for (int i = 0; i < samplePositions.Length; i++)
        {
            float d = Vector3.SqrMagnitude(worldPos - samplePositions[i]);
            if (d < bestDist)
            {
                bestDist = d;
                bestIdx = i;
            }
        }

        return sampleDistances[bestIdx];
    }

    private IEnumerator FollowSplineCoroutine()
    {
        float distanceAlong = FindClosestDistanceOnSpline(navMeshAgent.transform.position);
        currentDistanceAlong = distanceAlong;

        if (!navMeshAgent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(navMeshAgent.transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning("Agent is not on NavMesh and no nearby NavMesh found. Stopping spline follow.");
                yield break;
            }
        }

        while (true)
        {
            if (navMeshAgent == null || samplePositions == null || splineLength <= 0f)
                yield break;

            distanceAlong += navMeshAgent.speed * Time.deltaTime;
            currentDistanceAlong = distanceAlong;

            Vector3 targetPos = EvaluatePositionAtDistance(distanceAlong);

            Vector3 currentPos = navMeshAgent.transform.position;
            Vector3 smoothedPos = Vector3.Lerp(currentPos, targetPos, 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime));
            Vector3 displacement = smoothedPos - currentPos;

            navMeshAgent.Move(displacement);

            Vector3 lookPos = EvaluatePositionAtDistance(distanceAlong + lookAheadDistance);
            Vector3 desiredDir = lookPos - navMeshAgent.transform.position;
            desiredDir.y = 0f;

            if (desiredDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(desiredDir.normalized);
                navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, targetRot, rotationLerpSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (samplePositions != null && samplePositions.Length > 1)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < samplePositions.Length - 1; i++)
            {
                Gizmos.DrawLine(samplePositions[i], samplePositions[i + 1]);
            }

            Gizmos.color = Color.yellow;
            int step = Mathf.Max(1, samplePositions.Length / 30);
            for (int i = 0; i < samplePositions.Length; i += step)
            {
                Gizmos.DrawSphere(samplePositions[i], 0.1f);
            }
        }

        if (Application.isPlaying && samplePositions != null && samplePositions.Length > 0)
        {
            Vector3 tgt = EvaluatePositionAtDistance(currentDistanceAlong);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(tgt, 0.2f);

            if (navMeshAgent != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(navMeshAgent.transform.position, tgt);

                Vector3 lookPos = EvaluatePositionAtDistance(currentDistanceAlong + lookAheadDistance);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(tgt, lookPos);
            }
        }
    }
}
