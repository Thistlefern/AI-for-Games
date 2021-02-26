using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachineController : MonoBehaviour
{
    public Agent agent;

    public Vector3[] patrolWaypoints;
    public int currentPatrolIndex;

    public Transform intruderTransform;


    public enum States
    {
        Patrol, Seek, Attack
    }

    [SerializeField]
    private States currentState;

    private void Update()
    {
        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, 15.0f);

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

        if (Mathf.Abs(intruderTransform.position.x - transform.position.x) <= 8 || Mathf.Abs(intruderTransform.position.z - transform.position.z) <= 8)
        {
            ChangeState(States.Seek);
        }
    }
    void Seek()     // what happens while seeking
    {
        // TODO add seek movement

        if (Mathf.Abs(intruderTransform.position.x - transform.position.x) <= 4 || Mathf.Abs(intruderTransform.position.z - transform.position.z) <= 4)
        {
            ChangeState(States.Attack);
        }
        else if(Mathf.Abs(intruderTransform.position.x - transform.position.x) >= 12 || Mathf.Abs(intruderTransform.position.z - transform.position.z) >= 12)
        {
            ChangeState(States.Patrol);
        }
    }
    void Attack()   // what happens while attacking
    {
        // TODO add attack movement

        if (Mathf.Abs(intruderTransform.position.x - transform.position.x) > 8 || Mathf.Abs(intruderTransform.position.z - transform.position.z) > 8)
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
