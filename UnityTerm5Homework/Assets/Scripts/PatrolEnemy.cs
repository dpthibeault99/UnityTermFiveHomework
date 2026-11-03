using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        attackLayer = anim.GetLayerIndex("Attack");
        patrolling = true;
        lastPosition = transform.position;
    }
    
    bool CanSeePlayer()
    {
        if (Vector3.Distance(eye.position, target.position) < viewDistance)
        {
            Vector3 playerDirection = (target.position - eye.position).normalized;
            float angleDifference = Vector3.Angle(eye.forward, playerDirection);
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
        if (attacking) return;
        if (agent.pathPending)
        {
            return;
        }
        //Code for patrolling
        if (patrolling)
        {
            if(agent.remainingDistance < agent.stoppingDistance)
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
            if (!agent.isOnOffMeshLink && toPlayer.magnitude <= agent.stoppingDistance + 0.1f)
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
                if(agent.remainingDistance < agent.stoppingDistance)
                {
                    patrolling = true;
                    StartCoroutine(GoToNextPatrolPoint());
                }
            }
        }
        //play move animation
        anim.SetFloat("Moving", agent.velocity.sqrMagnitude);
    }

    IEnumerator AttackOnce()
    {
        attacking = true;
        previousIsStopped = agent.isStopped;
        previousUpdateRotation = agent.updateRotation;
        agent.isStopped = true;
        agent.updateRotation = false;
        anim.SetFloat("Moving", 0f);
        anim.SetBool("Attack", true);

        // Wait for the existing animation state, then let one swing complete.
        float entryDeadline = Time.time + 2f;
        while (!anim.GetCurrentAnimatorStateInfo(attackLayer).IsName("Standing Melee Attack Horizontal")
               && Time.time < entryDeadline)
            yield return null;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(attackLayer);
        if (!state.IsName("Standing Melee Attack Horizontal"))
            Debug.LogWarning("Attack animation did not start. Check the Attack layer and transitions.", this);

        while (state.IsName("Standing Melee Attack Horizontal") && state.normalizedTime < 1f)
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(attackLayer);
        }

        anim.SetBool("Attack", false);
        float exitDeadline = Time.time + 2f;
        while ((anim.IsInTransition(attackLayer) ||
                anim.GetCurrentAnimatorStateInfo(attackLayer).IsName("Standing Melee Attack Horizontal"))
               && Time.time < exitDeadline)
            yield return null;

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
            if (anim != null) anim.SetBool("Attack", false);
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
        if(waypoints.Length == 0)
        {
            yield break;
        }

        patrolling = true;
        yield return new WaitForSeconds(2.0f);
        // A patrol wait must not change the destination during combat.
        if (!patrolling || attacking) yield break;
        arrived = false;
        agent.destination = waypoints[destination].position;
        destination = (destination + 1) % waypoints.Length;
    }
} 
