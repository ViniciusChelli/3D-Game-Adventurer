using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCPerception3D))]
[RequireComponent(typeof(NPCInventory))]
[RequireComponent(typeof(SophiaNavMeshModule))]
public class NPCBrain3D : MonoBehaviour
{
    [Header("Interação")]
    public float interactionRange = 0.8f;
    public float repathInterval = 0.25f;

    [Header("Patrulha/Wander")]
    public List<Transform> patrolPoints = new List<Transform>();
    public bool useWanderWhenIdle = true;
    public Vector2 wanderAreaCenter;
    public Vector2 wanderAreaSize = new Vector2(8, 8);
    public float wanderPointMinDistance = 2f;

    [Header("Animação")]
    public string walkBoolParam = "isWalking";
    public Animator anim;

    private NPCPerception3D perception;
    private NPCInventory inventory;
    private SophiaNavMeshModule nav;
    private int patrolIndex;
    private Vector3 currentTarget;
    private IInteractable currentInteractable;
    private float repathTimer;

    private enum State { Idle, Patrol, Wander, MoveToTarget, Interact, Deposit }
    private State state = State.Idle;

    void Awake()
    {
        perception = GetComponent<NPCPerception3D>();
        inventory  = GetComponent<NPCInventory>();
        nav        = GetComponent<SophiaNavMeshModule>();
        if (!anim) anim = GetComponent<Animator>();
    }

    void OnEnable()  { nav.OnArrived += HandleArrived; }
    void OnDisable() { nav.OnArrived -= HandleArrived; }

    void Start()
    {
        nav.arriveThreshold = Mathf.Max(nav.arriveThreshold, interactionRange);

        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            patrolIndex = 0;
            currentTarget = patrolPoints[patrolIndex].position;
            Go(currentTarget);
            state = State.Patrol;
        }
        else
        {
            state = useWanderWhenIdle ? State.Wander : State.Idle;
            if (state == State.Wander)
            {
                currentTarget = NextWanderPoint3D();
                Go(currentTarget);
            }
        }
    }

    void Update()
    {
        SetWalking(nav.agent && nav.agent.velocity.sqrMagnitude > 0.01f);

        if (state != State.Deposit && inventory.IsFull())
        {
            var chest = perception.FindClosestStorage(transform.position);
            if (chest != null)
            {
                currentInteractable = chest;
                currentTarget = chest.GetWorldPosition();
                state = State.Deposit;
                Go(currentTarget);
                return;
            }
        }

        if (state == State.MoveToTarget || state == State.Deposit)
        {
            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                if (currentInteractable == null || !currentInteractable.IsValid()) { Fallback(); return; }
                currentTarget = currentInteractable.GetWorldPosition();
                Go(currentTarget);
                repathTimer = repathInterval;
            }

            if (DistXZ(transform.position, currentTarget) <= interactionRange) StartInteraction();
            return;
        }

        if (state != State.Interact)
        {
            var seen = perception.FindBestTarget(transform.position, inventory);
            if (seen != null)
            {
                currentInteractable = seen;
                currentTarget = seen.GetWorldPosition();
                state = State.MoveToTarget;
                Go(currentTarget);
                return;
            }
        }

        if (state == State.Wander && nav.agent && !nav.agent.pathPending && nav.agent.isStopped)
        {
            currentTarget = NextWanderPoint3D();
            Go(currentTarget);
        }
    }

    void HandleArrived(Vector3 dest)
    {
        switch (state)
        {
            case State.Patrol:
                patrolIndex = (patrolIndex + 1) % Mathf.Max(1, patrolPoints.Count);
                currentTarget = patrolPoints[patrolIndex].position;
                Go(currentTarget);
                break;

            case State.Wander:
                currentTarget = NextWanderPoint3D();
                Go(currentTarget);
                break;

            case State.MoveToTarget:
            case State.Deposit:
                StartInteraction();
                break;
        }
    }

    void StartInteraction()
    {
        if (currentInteractable == null || !currentInteractable.IsValid()) { Fallback(); return; }
        if (DistXZ(transform.position, currentInteractable.GetWorldPosition()) > interactionRange)
        {
            Go(currentInteractable.GetWorldPosition()); return;
        }
        StartCoroutine(InteractRoutine());
    }

    IEnumerator InteractRoutine()
    {
        state = State.Interact;
        SetWalking(false);

        float wait = Mathf.Max(0f, currentInteractable.InteractionTime());
        if (wait > 0f) yield return new WaitForSeconds(wait);

        currentInteractable.Interact(this, inventory);
        currentInteractable = null;

        if (inventory.IsFull())
        {
            var chest = perception.FindClosestStorage(transform.position);
            if (chest)
            {
                currentInteractable = chest;
                currentTarget = chest.GetWorldPosition();
                state = State.Deposit;
                Go(currentTarget);
                yield break;
            }
        }
        Fallback();
    }

    void Fallback()
    {
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            state = State.Patrol;
            currentTarget = patrolPoints[patrolIndex].position;
            Go(currentTarget);
        }
        else if (useWanderWhenIdle)
        {
            state = State.Wander;
            currentTarget = NextWanderPoint3D();
            Go(currentTarget);
        }
        else
        {
            state = State.Idle;
            if (nav.agent) nav.agent.isStopped = true;
            SetWalking(false);
        }
    }

    void Go(Vector3 pos)
    {
        repathTimer = repathInterval;
        nav.ResumeAfterTalk();
        nav.GoTo(pos);
    }

    void SetWalking(bool w) { if (anim) anim.SetBool(walkBoolParam, w); }

    Vector3 NextWanderPoint3D()
    {
        Vector2 half = wanderAreaSize * 0.5f;
        Vector2 p; int guard = 0;
        do
        {
            p = new Vector2(
                Random.Range(wanderAreaCenter.x - half.x, wanderAreaCenter.x + half.x),
                Random.Range(wanderAreaCenter.y - half.y, wanderAreaCenter.y + half.y)
            );
            guard++;
        } while (Vector2.Distance(p, new Vector2(transform.position.x, transform.position.z)) < wanderPointMinDistance && guard < 12);
        return new Vector3(p.x, transform.position.y, p.y);
    }

    static float DistXZ(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }
}
