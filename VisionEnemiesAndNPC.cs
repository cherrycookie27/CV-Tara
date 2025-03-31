using System.Collections;
using UnityEngine;

public class VisionEnemiesAndNPC : MonoBehaviour
{
    public GameObject player;
    public GameObject alarmed;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Vision field")]
    public float priorityRadius;
    public float radius;
    [Range(0, 360)] public float angle;

    [Header("Alert field")]
    public float alertRadius;
    [Range(0, 360)] public float alertAngle;
    public float soundAlert;

    [Header("Speeds")]
    public float alertBreak = 3f;
    public float turnSpeed = 180f;

    [Header("Testing")]
    public bool canSeeTarget;
    public bool alerted;

    private bool turningToAlertedPosition = false;
    public Transform currentTarget;
    private Vector3 lastKnownLocation;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
        alarmed.SetActive(false);
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            RotateTowards(currentTarget.position);
        }
        else if (turningToAlertedPosition)
        {
            RotateTowards(lastKnownLocation);
        }
    }

    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    void FieldOfViewCheck()
    {
        canSeeTarget = false;
        currentTarget = null;

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        Transform closestNonPlayerTarget = null;
        float closestNonPlayerDistance = float.MaxValue;

        for (int i = 0; i < rangeChecks.Length; i++)
        {
            Collider targetCollider = rangeChecks[i];
            Transform target = targetCollider.transform;
            if (target == transform) continue;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2 && distanceToTarget <= radius)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    //make it (later when done w damage for player) prioritise target close to it dealing damage to it

                    // Prioritize player if theyre within priority radius
                    if (target.CompareTag("Player") && distanceToTarget <= priorityRadius)
                    {
                        currentTarget = target;
                        canSeeTarget = true;
                        turningToAlertedPosition = false;
                        return;
                    }

                    // Track closest target if no player close
                    if (distanceToTarget < closestNonPlayerDistance)
                    {
                        closestNonPlayerDistance = distanceToTarget;
                        closestNonPlayerTarget = target;
                    }
                }
            }
        }

        // If no player in priority radius, follow the closest target
        if (closestNonPlayerTarget != null)
        {
            currentTarget = closestNonPlayerTarget;
            canSeeTarget = true;
            turningToAlertedPosition = false;
        }

        // Alert field
        Collider[] alertChecks = Physics.OverlapSphere(transform.position, alertRadius, targetMask);
        foreach (Collider targetCollider in alertChecks)
        {
            Transform target = targetCollider.transform;
            if (target == transform) continue;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (Vector3.Angle(transform.forward, directionToTarget) < alertAngle / 2 && distanceToTarget <= alertRadius)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if (!alerted)
                    {
                        alerted = true;
                        lastKnownLocation = target.position;
                        StartCoroutine(Alerted());
                    }
                    return;
                }
            }
            else if (distanceToTarget < soundAlert)
            {
                if (!alerted)
                {
                    alerted = true;
                    lastKnownLocation = target.position;
                    StartCoroutine(Alerted());
                }
                return;
            }
        }
    }

    IEnumerator Alerted()
    {
        if (!canSeeTarget)
        {
            alarmed.SetActive(true);
            yield return new WaitForSeconds(2f);

            turningToAlertedPosition = true;
            yield return new WaitUntil(() => !turningToAlertedPosition);

            yield return new WaitForSeconds(alertBreak);
            alarmed.SetActive(false);
            alerted = false;
        }
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            turningToAlertedPosition = false;
        }
    }
}
