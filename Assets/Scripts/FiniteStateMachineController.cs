using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachineController : MonoBehaviour
{
    public Agent fsmAgent;

    public Vector3[] patrolWaypoints;
    public int currentPatrolIndex;

    public Transform intruderTransform;

    public bool intruderDetected = false;
    public bool inAttackRange = false;

    public enum States
    {
        Patrol, Seek, Attack
    }

    [SerializeField]
    private States currentState;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 4.0f);
        Gizmos.DrawWireSphere(transform.position, 8.0f);
    }

    private void Update()
    {
        Collider[] attackRadius = Physics.OverlapSphere(transform.position, 4.0f);
        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, 8.0f);

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
    }
    void Patrol()   // what happens while patrolling
    {
        // TODO add patrol movement

        if (intruderDetected)
        {
            ChangeState(States.Seek);
        }
    }
    void Seek()     // what happens while seeking
    {
        // TODO add seek movement

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
        // TODO add attack movement

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
