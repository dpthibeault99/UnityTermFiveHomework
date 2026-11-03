using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolEnemy : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform[] waypoints;
    bool arrived;
    bool patrolling;
    int destination;
    public Transform eye;
    public Transform target;
    Vector3 lastPosition;
    public float viewDistance = 10.0f;
    public float viewAngle = 90.0f;
    public LayerMask playerMask;
    public Animator anim;
    [Min(0f)] public float attackRecovery = 0.5f;
    [Range(0f, 180f)] public float attackFacingAngle = 20f;
    [Min(0f)] public float combatTurnSpeed = 360f;
    bool attacking;
    float nextAttackTime;
    int attackLayer;
    bool previousUpdateRotation;
    bool previousIsStopped;
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (anim == null || anim.runtimeAnimatorController == null || !agent.isOnNavMesh)
        {
            Debug.LogError("Enemy needs an Animator with a controller and a NavMeshAgent on the baked NavMesh.", this);
            enabled = false;
            return;
        }
        attackLayer = anim.GetLayerIndex("Attack");
        patrolling = true;
        lastPosition = transform.position;
        agent.isStopped = false;
        SetNextPatrolDestination();
    }
    
    bool CanSeePlayer()
    {
        if (eye == null || target == null) return false;
        if (Vector3.Distance(eye.position, target.position) < viewDistance)
        {
            // Field of view is horizontal; looking down at a nearby player must not lose them.
            Vector3 playerDirection = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up);
            float angleDifference = Vector3.Angle(Vector3.ProjectOnPlane(eye.forward, Vector3.up), playerDirection);
            if (angleDifference < viewAngle /2)
            {
                
                if (!Physics.Linecast(eye.position, target.position, ~playerMask))
                {
                    //enemy sees player
                    lastPosition = target.position;
                    return true;
                }

            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        if (attacking) return;
        anim.SetFloat("Moving", agent.velocity.sqrMagnitude);
        if (agent.pathPending)
        {
            return;
        }
        //Code for patrolling
        if (patrolling)
        {
            if(agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.1f))
            {
                if (!arrived)
                {
                    arrived = true;
                    //Start Coroutine
                    StartCoroutine(GoToNextPatrolPoint());
                }
            }
        }
        else
        {
            arrived = false;
        }
        //Code for when enemy sees target
        if (CanSeePlayer())
        {
            agent.SetDestination(target.transform.position);
            patrolling = false;
            // Use player distance, not a path that may still be recalculating.
            Vector3 toPlayer = target.position - transform.position;
            Vector3 facingDirection = Vector3.ProjectOnPlane(toPlayer, Vector3.up);
            if (!agent.isOnOffMeshLink && facingDirection.magnitude <= agent.stoppingDistance + 0.1f)
            {
                if (facingDirection.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.LookRotation(facingDirection), combatTurnSpeed * Time.deltaTime);
                }

                if (Time.time >= nextAttackTime && attackLayer >= 0 &&
                    Vector3.Angle(transform.forward, facingDirection) <= attackFacingAngle)
                {
                    StartCoroutine(AttackOnce());
                }
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }
        else
        {
            if (!patrolling)
            {
                anim.SetBool("Attack", false);
                agent.SetDestination(lastPosition);
                if(agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.1f))
                {
                    patrolling = true;
                    arrived = true;
                    StartCoroutine(GoToNextPatrolPoint());
                }
            }
        }
    }

    IEnumerator AttackOnce()
    {
        attacking = true;
        previousIsStopped = agent.isStopped;
        previousUpdateRotation = agent.updateRotation;
        agent.isStopped = true;
        agent.updateRotation = false;
        anim.SetFloat("Moving", 0f);
        anim.SetLayerWeight(attackLayer, 1f);
        anim.SetBool("Attack", true);

        // Wait for the existing animation state, then let one swing complete.
        float entryDeadline = Time.time + 2f;
        while (!anim.GetCurrentAnimatorStateInfo(attackLayer).IsName("Attack")
               && Time.time < entryDeadline)
            yield return null;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(attackLayer);
        if (!state.IsName("Attack"))
            Debug.LogWarning("Attack animation did not start. Check the Attack layer and transitions.", this);

        while (state.IsName("Attack") && state.normalizedTime < 1f)
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(attackLayer);
        }

        anim.SetBool("Attack", false);
        float exitDeadline = Time.time + 2f;
        while ((anim.IsInTransition(attackLayer) ||
                anim.GetCurrentAnimatorStateInfo(attackLayer).IsName("Attack"))
               && Time.time < exitDeadline)
            yield return null;

        // Let the base layer animate the legs again after the swing.
        anim.SetLayerWeight(attackLayer, 0f);
        agent.isStopped = previousIsStopped;
        agent.updateRotation = previousUpdateRotation;
        nextAttackTime = Time.time + attackRecovery;
        attacking = false;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (attacking)
        {
            if (anim != null)
            {
                anim.SetBool("Attack", false);
                if (attackLayer >= 0) anim.SetLayerWeight(attackLayer, 0f);
            }
            if (agent != null)
            {
                agent.updateRotation = previousUpdateRotation;
                if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                    agent.isStopped = previousIsStopped;
            }
        }
        attacking = false;
        arrived = false;
    }

    IEnumerator GoToNextPatrolPoint()
    {
        if(waypoints == null || waypoints.Length == 0)
        {
            yield break;
        }

        patrolling = true;
        yield return new WaitForSeconds(2.0f);
        // A patrol wait must not change the destination during combat.
        if (!patrolling || attacking) yield break;
        arrived = false;
        SetNextPatrolDestination();
    }

    void SetNextPatrolDestination()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform waypoint = waypoints[destination];
            destination = (destination + 1) % waypoints.Length;
            if (waypoint == null) continue;
            agent.SetDestination(waypoint.position);
            return;
        }
    }
} 
