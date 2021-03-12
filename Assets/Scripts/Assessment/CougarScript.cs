using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CougarScript : MonoBehaviour
{
    public Agent cougarAgent;
    public float speed = 8.0f;
    public float health = 40.0f;
    public float fatigue = 0.0f;
    public float thirst = 0.0f;
    public float hunger = 40.0f;
    public float perception = 20.0f;
    public float sneak;
    public bool isSneaking;
    public float foodLeft = 0.0f;
    // TODO maybe add cache of where food is stored?

    public float reachedThreshold = 0.5f;

    public float timer = 0.0f;

    public Transform intruderTransform;
    public Transform lake;

    public bool intruderDetected = false;
    public bool inAttackRange = false;

    public float attackRange = 4.0f;
    public float seekDistance = 10.0f;

    static Vector3 randomTarget;

    public enum States
    {
        Wander, Sleep, Search, Drink, Eat, Sneak, Chase
    }

    [SerializeField]
    private States currentState;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, perception);
    }

    private void Update()
    {
        timer++;
        if(timer == 2.0f)
        {
            fatigue += 0.5f;
            hunger++;
            thirst++;
            timer = 0.0f;
        }

        Collider[] attackRadius = Physics.OverlapSphere(transform.position, attackRange);
        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, perception);

        foreach (var hitCollider in detectionRadius)
        {
            if (hitCollider.name != name)
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
            case States.Sleep:
                Sleep();
                break;
            case States.Search:
                Search();
                break;
            case States.Drink:
                Drink();
                break;
            case States.Eat:
                Eat();
                break;
            case States.Sneak:
                Sneak();
                break;
            case States.Chase:
                Chase();
                break;
            default:
                Wander();
                break;
        }

        intruderDetected = false;
        inAttackRange = false;
    }
    void Sleep()
    {
        if(timer == 2.0f)
        {
            fatigue -= 1.5f;
        }
        
        if(fatigue <= 50)
        {
            if(thirst >= 80)
            {
                ChangeState(States.Search);
            }
            else if(hunger >= 80)
            {
                ChangeState(States.Search);
            }
        }
        else if(fatigue <= 0)
        {
            fatigue = 0;
            ChangeState(States.Wander);
        }
    }
    void Search()
    {
        if(hunger > thirst)
        {
            if(foodLeft != 0)
            {
                ChangeState(States.Eat);
            }
            else
            {
                // TODO search for food, switch to sneak when prey is located
            }
        }
        else
        {
            // TODO search for water
            // TODO implement stopping search when water is reached
        }
    }
    void Drink()
    {
        if(timer == 2.0f)
        {
            thirst -= 6.0f;
        }

        if(thirst <= 40)
        {
            if(hunger >= 80)
            {
                ChangeState(States.Search);
            }
        }
        else if (thirst <= 0)
        {
            ChangeState(States.Wander);
        }
    }
    void Eat()
    {
        if (timer == 2.0f)
        {
            foodLeft--;
            hunger -= 6.0f;
        }

        if (hunger <= 40)
        {
            if (thirst >= 80)
            {
                ChangeState(States.Search);
            }
        }
        else if (hunger <= 0)
        {
            ChangeState(States.Wander);
        }
    }
    void Sneak()
    {
        if (!isSneaking)
        {
            System.Random random = new System.Random();
            sneak = random.Next(10);
            isSneaking = true;
        }
        else
        {
            // TODO move towards prey
            // TODO when close enough, chase
        }
    }
    void Chase()
    {
        // TODO attack
        // TODO chase after prey
        // TODO if prey exits area of perception, search again
        // TODO add attack, and add 50 to foodLeft if prey is killed
    }
    void Wander()
    {
        // TODO add wander

        if (fatigue >= 50)
        {
            ChangeState(States.Sleep);
        }
        else if (thirst >= 50)
        {
            ChangeState(States.Search);
        }
        else if (hunger >= 50)
        {
            ChangeState(States.Search);
        }
    }

    //void Seek()     // what happens while seeking
    //{
    //    timer += Time.deltaTime;

    //    if (timer >= waitInterval)
    //    {
    //        randomTarget = (Random.insideUnitSphere * seekDistance) + intruderTransform.position;
    //        timer = 0.0f;
    //    }
    //    randomTarget.y = 0;

    //    Vector3 offset = randomTarget - transform.position;

    //    cougarAgent.velocity = offset.normalized * speed;
    //    cougarAgent.UpdateMovement();


    //    if (inAttackRange)
    //    {
    //        ChangeState(States.Attack);
    //    }
    //    else if (!intruderDetected)
    //    {
    //        ChangeState(States.Patrol);
    //    }
    //}
    //void Attack()   // what happens while attacking
    //{
    //    Vector3 offset = intruderTransform.position - transform.position;

    //    cougarAgent.velocity = offset.normalized * speed;
    //    cougarAgent.UpdateMovement();

    //    if (!inAttackRange)
    //    {
    //        ChangeState(States.Seek);
    //    }
    //}

    void ChangeState(States newState)
    {
        if (newState == States.Patrol)
        {
            OnSeekExit();
            OnPatrolEnter();
        }
        else if (newState == States.Attack)
        {
            OnSeekExit();
            OnAttackEnter();
        }
        else if (newState == States.Seek)
        {
            if (currentState == States.Patrol)
            {
                OnPatrolExit();
            }
            else if (currentState == States.Attack)
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
