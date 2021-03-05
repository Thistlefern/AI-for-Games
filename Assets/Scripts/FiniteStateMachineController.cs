using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachineController : MonoBehaviour
{
    public Agent fsmAgent;
    public float speed = 3.0f;

    public Transform[] waypoints;
    private int currentWaypointID = 0;

    public float reachedThreshold = 0.5f;

    public float waitInterval = 2.0f;      // amount of time to wait at each patrol waypoint in seconds
    public float waitTimer = 2.0f;

    public Transform intruderTransform;

    public bool intruderDetected = false;
    public bool inAttackRange = false;

    public float perceptionDistance = 8.0f;
    public float attackRange = 4.0f;
    public float seekDistance = 10.0f;

    static Vector3 randomTarget;

    public enum States
    {
        Patrol, Seek, Attack
    }

    [SerializeField]
    private States currentState;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, perceptionDistance);
    }

    private void Update()
    {
        Collider[] attackRadius = Physics.OverlapSphere(transform.position, attackRange);
        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, perceptionDistance);


        foreach (var hitCollider in detectionRadius)
        {
            if(hitCollider.name != name)
            {
                intruderDetected = true;
            }
        }

        foreach (var hitCollider in attackRadius)
        {
            if (hitCollider.name != name)
            {
                inAttackRange = true;
            }
        }

        switch (currentState)
        {
            case States.Patrol: // when currentstate is Patrol...
                Patrol();       // ...run the Patrol function
                break;
            case States.Seek:   // when currentstate is Seek...
                Seek();         // ...run the Seek function
                break;
            case States.Attack: // when currentstate is Attack...
                Attack();       // ...run the Attack function
                break;
            default:
                Debug.LogError("Invalid state!");
                break;
        }

        intruderDetected = false;
        inAttackRange = false;
    }
    void Patrol()   // what happens while patrolling
    {
        waitTimer += Time.deltaTime;

        if (waitTimer < waitInterval)
        {
            return;
        }
        else
        {

            Vector3 offset = waypoints[currentWaypointID].position - transform.position;

            fsmAgent.velocity = offset.normalized * speed;

            fsmAgent.UpdateMovement();

            if (offset.magnitude <= reachedThreshold)    // checks if patrol has reached waypoint
            {
                waitTimer = 0;              // set timer to zero to begin counting time at waypoint
                currentWaypointID++;        // if waypoint was reached, target waypoint becomes next waypoint
                if (currentWaypointID >= waypoints.Length)   // loop back to the beginning after reaching the last waypoint
                {
                    currentWaypointID = 0;
                }
            }
        }

        if (intruderDetected)
        {
            ChangeState(States.Seek);
        }
    }
    void Seek()     // what happens while seeking
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitInterval)
        {
            randomTarget = (Random.insideUnitSphere * seekDistance) + intruderTransform.position;
            waitTimer = 0.0f;
        }
        randomTarget.y = 0;

        Vector3 offset = randomTarget - transform.position;

        fsmAgent.velocity = offset.normalized * speed;
        fsmAgent.UpdateMovement();


        if (inAttackRange)
        {
            ChangeState(States.Attack);
        }
        else if(!intruderDetected)
        {
            ChangeState(States.Patrol);
        }
    }
    void Attack()   // what happens while attacking
    {
        Vector3 offset = intruderTransform.position - transform.position;

        fsmAgent.velocity = offset.normalized * speed;
        fsmAgent.UpdateMovement();

        if (!inAttackRange)
        {
            ChangeState(States.Seek);
        }
    }

    void ChangeState(States newState)
    {
        if(newState == States.Patrol)
        {
            OnSeekExit();
            OnPatrolEnter();
        }
        else if(newState == States.Attack)
        {
            OnSeekExit();
            OnAttackEnter();
        }
        else if(newState == States.Seek)
        {
            if(currentState == States.Patrol)
            {
                OnPatrolExit();
            }
            else if(currentState == States.Attack)
            {
                OnAttackExit();
            }
            OnSeekEnter();
        }
    }

    void OnPatrolEnter()
    {
        Debug.Log("I guess it was nothing...");
        currentState = States.Patrol;
    }
    void OnPatrolExit()
    {
        Debug.Log("What was that?");
    }
    
    void OnSeekEnter()
    {
        Debug.Log("Where are you?");
        currentState = States.Seek;
    }
    void OnSeekExit()
    {
        // seek concludes, didn't add a Debug.Log because there are two reasons why a seek would end and no quote made sense for both
    }  
    
    void OnAttackEnter()
    {
        Debug.Log("There you are!");
        currentState = States.Attack;
    }
    void OnAttackExit()
    {
        Debug.Log("Get back here!");
    }

}
