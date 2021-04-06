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

    public float reachedThreshold = 0.5f;

    public float timer = 0.0f;
    public float wanderTimer = 0.0f;
    public float timeSpentWandering = 3.0f;

    public Transform intruderTransform;
    public Transform lake;

    public Vector3 intruderLastLocation;
    public Vector3 food;

    public bool preyDetected = false;
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
        if (currentState != States.Sneak)
        {
            speed = 8;
        }

        if(intruderTransform.position == intruderLastLocation && inAttackRange)
        {
            food = intruderTransform.position;
        }

        preyDetected = false;
        inAttackRange = false;

        timer += Time.deltaTime;

        Collider[] attackRadius = Physics.OverlapSphere(transform.position, attackRange);
        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, perception);

        foreach (var hitCollider in detectionRadius)
        {
            if (hitCollider.name == intruderTransform.name)
            {
                preyDetected = true;
            }
        }

        foreach (var hitCollider in attackRadius)
        {
            if (hitCollider.name == intruderTransform.name)
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

        if (timer >= 2.0f)
        {
            fatigue += 0.5f;
            hunger++;
            thirst++;
            timer = 0.0f;
        }
        intruderLastLocation = intruderTransform.position;
    }
    void Sleep()
    {
        if(timer >= 2.0f)
        {
            fatigue -= 2.5f;
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
            if(food != intruderTransform.position)
            {
                Vector3 offset = intruderTransform.position - transform.position;
                cougarAgent.velocity = offset.normalized * speed;
                cougarAgent.UpdateMovement();

                if (preyDetected)
                {
                    ChangeState(States.Sneak);
                }
            }
            else
            {
                Vector3 offset = food - transform.position;
                cougarAgent.velocity = offset.normalized * speed;
                cougarAgent.UpdateMovement();

                if (offset.magnitude <= reachedThreshold)
                {
                    ChangeState(States.Eat);
                }
            }
        }
        else
        {
            Vector3 offset = lake.position - transform.position;
            cougarAgent.velocity = offset.normalized * speed;
            cougarAgent.UpdateMovement();

            if (offset.magnitude <= reachedThreshold)
            {
                ChangeState(States.Drink);
            }
        }
    }
    void Drink()
    {
        if(timer >= 2.0f)
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
        if (timer >= 2.0f)
        {
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
        speed = 4;
        Vector3 offset = intruderTransform.position - transform.position;
        cougarAgent.velocity = offset.normalized * speed;
        cougarAgent.UpdateMovement();

        if(food == intruderTransform.position)
        {
            ChangeState(States.Eat);
        }
        else if(transform.position.x - intruderTransform.position.x > 5 || transform.position.y - intruderTransform.position.y > 5)
        {
            ChangeState(States.Chase);
        }
    }

    void Chase()
    {
        Vector3 offset = intruderTransform.position - transform.position;
        cougarAgent.velocity = offset.normalized * speed;
        cougarAgent.UpdateMovement();

        if (food == intruderTransform.position)
        {
            ChangeState(States.Eat);
        }
        else if (!preyDetected)
        {
            ChangeState(States.Search);
        }
    }
    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer > timeSpentWandering)
        {
            randomTarget = (Random.insideUnitSphere * seekDistance) + transform.position;
            wanderTimer = 0.0f;
        }
        randomTarget.y = 0;

        Vector3 offset = randomTarget - transform.position;

        cougarAgent.velocity = offset.normalized * speed;
        cougarAgent.UpdateMovement();

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

    void ChangeState(States newState)
    {
        switch (currentState)
        {
            case States.Sleep:
                OnSleepExit();
                if(newState == States.Search)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnWanderEnter();
                }
                break;
            case States.Search:
                OnSearchExit();
                if(newState == States.Eat)
                {
                    OnEatEnter();
                }
                else if(newState == States.Sneak)
                {
                    OnSneakEnter();
                }
                else
                {
                    OnDrinkEnter();
                }
                break;
            case States.Drink:
                OnDrinkExit();
                if(newState == States.Search)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnWanderEnter();
                }
                break;
            case States.Eat:
                OnEatExit();
                if (newState == States.Search)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnWanderEnter();
                }
                break;
            case States.Sneak:
                OnSneakExit();
                OnChaseEnter();
                break;
            case States.Chase:
                OnChaseExit();
                if(food != intruderTransform.position)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnEatEnter();
                }
                break;
            case States.Wander:
                OnWanderExit();
                if(newState == States.Search)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnSleepEnter();
                }
                break;
        }
    }

    // Sleep
    void OnSleepEnter()
    {
        // anything that happens when sleep state starts
        currentState = States.Sleep;
    }
    void OnSleepExit()
    {
        // anything that happens when sleep state ends
    }

    // Search
    void OnSearchEnter()
    {
        // anything that happens when search state starts
        currentState = States.Search;
    }
    void OnSearchExit()
    {
        // anything that happens when search state ends
    }

    // Drink
    void OnDrinkEnter()
    {
        // anything that happens when drink state starts
        currentState = States.Drink;
    }
    void OnDrinkExit()
    {
        // anything that happens when drink state ends
    }

    // Eat
    void OnEatEnter()
    {
        // anything that happens when eat state starts
        currentState = States.Eat;
    }
    void OnEatExit()
    {
        // anything that happens when eat state ends
    }

    // Sneak
    void OnSneakEnter()
    {
        // anything that happens when sneak state starts
        currentState = States.Sneak;
    }
    void OnSneakExit()
    {
        // anything that happens when sneak state ends
    }

    // Chase
    void OnChaseEnter()
    {
        currentState = States.Chase;
    }
    void OnChaseExit()
    {

    }

    // Wander
    void OnWanderEnter()
    {
        // anything that happens when wander state starts
        currentState = States.Wander;
    }
    void OnWanderExit()
    {
        // anything that happens when wander state ends
    }
}
