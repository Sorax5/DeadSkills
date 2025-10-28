using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

[RequireComponent(typeof(NavMeshAgent))]
public class CarMovement : MonoBehaviour
{
    [Header("Spline Settings")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float followDistance = 2f; // distance réelle à viser le long de la spline (mètres)
    [SerializeField, Range(0.001f, 0.25f)] private float lookAhead = 0.02f; // demi-fenêtre locale autour de t courant (en t normalisé)
    [SerializeField] private bool loop = true; // si false, on pince entre [0,1] et on ne wrap pas

    [Header("Movement Settings")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float sampleRadius = 2f;
    [SerializeField] private float updateInterval = 0.2f;

    private NavMeshAgent agent;
    private bool isMoving;
    private float splineLength;
    private float splineProgress;
    private float nextUpdateTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // On gère la rotation
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

        // Initialiser la progression au point le plus proche pour ancrer le suivi local
        splineProgress = FindClosestSplineTGlobal(transform.position);

        if (autoStart)
            StartMovement();
    }

    private void Update()
    {
        if (!isMoving || splineContainer == null)
            return;

        // Attendre un intervalle pour éviter de spammer SetDestination()
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            UpdateTargetOnSpline();
        }

        // Gestion manuelle de la rotation vers la direction du déplacement
        if (agent.desiredVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.desiredVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * agent.angularSpeed / 10f);
        }
    }

    private void UpdateTargetOnSpline()
    {
        // 1) Rechercher le point le plus proche mais uniquement localement autour de la progression courante
        Vector3 worldPos = transform.position;
        float closestLocalT = FindClosestSplineTAround(worldPos, splineProgress, lookAhead);

        // 2) Avancer le long de la spline d'une distance réelle (followDistance)
        float targetT = AdvanceAlongSplineByDistance(closestLocalT, followDistance);

        // 3) Gérer le bouclage ou le pincement
        targetT = loop ? Mathf.Repeat(targetT, 1f) : Mathf.Clamp01(targetT);
        splineProgress = targetT;

        // 4) Évaluer la position cible sur la spline (monde)
        Vector3 splineTarget = EvaluateWorldPosition(splineProgress);

        // 5) Projeter la cible sur le NavMesh (sans warp)
        if (NavMesh.SamplePosition(splineTarget, out var hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Recherche globale (utilisée au démarrage uniquement)
    private float FindClosestSplineTGlobal(Vector3 worldPos)
    {
        var spline = splineContainer.Spline;
        Transform t = splineContainer.transform;

        int samples = 64;
        float bestT = 0f;
        float bestDist = float.MaxValue;

        for (int i = 0; i <= samples; i++)
        {
            float tValue = (float)i / samples;
            Vector3 p = t.TransformPoint(spline.EvaluatePosition(tValue));
            float d = Vector3.SqrMagnitude(worldPos - p);
            if (d < bestDist)
            {
                bestDist = d;
                bestT = tValue;
            }
        }

        return bestT;
    }

    // Recherche locale autour d'un t donné, avec gestion du wrap
    private float FindClosestSplineTAround(Vector3 worldPos, float aroundT, float halfWindow)
    {
        var spline = splineContainer.Spline;
        Transform tr = splineContainer.transform;

        int samples = 32;
        float bestT = aroundT;
        float bestDist = float.MaxValue;

        for (int i = 0; i <= samples; i++)
        {
            float alpha = (float)i / samples; // [0..1]
            float offset = Mathf.Lerp(-halfWindow, halfWindow, alpha);
            float tValue = loop ? Mathf.Repeat(aroundT + offset, 1f)
                                : Mathf.Clamp01(aroundT + offset);

            Vector3 p = tr.TransformPoint(spline.EvaluatePosition(tValue));
            float d = Vector3.SqrMagnitude(worldPos - p);
            if (d < bestDist)
            {
                bestDist = d;
                bestT = tValue;
            }
        }

        return bestT;
    }

    // Avance le paramètre t d'une distance réelle (approximation adaptative en quelques itérations)
    private float AdvanceAlongSplineByDistance(float startT, float distance)
    {
        if (distance <= 0f || splineLength <= 0f)
            return startT;

        float t = startT;
        Vector3 prev = EvaluateWorldPosition(t);

        // Estimation initiale du pas en t (grossière)
        float stepT = Mathf.Clamp(distance / Mathf.Max(splineLength, 0.0001f), 0.0025f, 0.1f);

        float moved = 0f;
        int safety = 0;
        const int maxIter = 16;

        while (moved < distance && safety++ < maxIter)
        {
            float nextT = loop ? Mathf.Repeat(t + stepT, 1f) : Mathf.Clamp01(t + stepT);
            Vector3 nextPos = EvaluateWorldPosition(nextT);

            float seg = Vector3.Distance(prev, nextPos);
            if (seg <= 1e-4f)
                break;

            moved += seg;
            t = nextT;
            prev = nextPos;

            float remaining = distance - moved;
            // Ajuster dynamiquement le pas en t selon la "vitesse" locale (mètre / t)
            float metersPerT = seg / stepT;
            stepT = Mathf.Clamp(remaining / Mathf.Max(metersPerT, 1e-4f), 0.001f, 0.1f);

            if (!loop && Mathf.Approximately(t, 1f)) // si pas de boucle, on peut s'arrêter en bout
                break;
        }

        return t;
    }

    private Vector3 EvaluateWorldPosition(float t)
    {
        var local = splineContainer.Spline.EvaluatePosition(t);
        return splineContainer.transform.TransformPoint(local);
    }

    public void StartMovement() => isMoving = true;
    public void StopMovement() => isMoving = false;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (splineContainer == null) return;
        var spline = splineContainer.Spline;

        Gizmos.color = Color.cyan;
        int steps = 50;
        Vector3 prev = splineContainer.transform.TransformPoint(spline.EvaluatePosition(0f));

        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector3 pos = splineContainer.transform.TransformPoint(spline.EvaluatePosition(t));
            Gizmos.DrawLine(prev, pos);
            prev = pos;
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector3 current = splineContainer.transform.TransformPoint(spline.EvaluatePosition(splineProgress));
            Gizmos.DrawSphere(current, 0.15f);
        }
    }
#endif
}
