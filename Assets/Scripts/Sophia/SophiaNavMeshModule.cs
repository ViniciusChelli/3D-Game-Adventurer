using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class SophiaNavMeshModule : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;

    [Header("Estado")]
    public bool isConversing = false;

    public event Action<Vector3> OnArrived;
    public Vector3 CurrentDestination { get; private set; }
    public float arriveThreshold = 0.35f;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;  // top-down
        agent.updateUpAxis   = false;
    }

    void Update()
    {
        if (isConversing) { agent.isStopped = true; return; }

        if (agent.isOnNavMesh && !agent.pathPending && !agent.isStopped &&
            agent.remainingDistance <= arriveThreshold)
        {
            var dest = CurrentDestination;
            agent.isStopped = true;
            OnArrived?.Invoke(dest);
        }
    }

    public void GoTo(Vector3 pos)
    {
        if (!agent.isOnNavMesh) return;
        CurrentDestination = pos;
        agent.isStopped = false;
        agent.SetDestination(pos);
    }

    public void InterruptAndTalk() { isConversing = true;  agent.isStopped = true; }
    public void ResumeAfterTalk()  { isConversing = false; agent.isStopped = false; }
}
