using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaDeerScript : MonoBehaviour
{
    public Agent alphaAgent;
    public float speed = 10.0f;
    public float health = 40.0f;
    public float fatigue = 0.0f;
    public float thirst = 0.0f;
    public float hunger = 40.0f;
    public float perception = 20.0f;
    public float cougarSneak = 0.0f;
    public bool cougarIsSneaking;
    public bool waterInRange;

    public float reachedThreshold = 5.0f;

    public float timer = 0.0f;
    public float sneakTimer = 0.0f;
    [SerializeField]
    public float wanderTimer = 0.0f;
    public float timeSpentWandering = 3.0f;
    public float fleeTimer = 0.0f;
    public float timeSpentFleeing = 3.0f;
    public float timeSinceAttacked = 0.0f;


    public Transform predatorTransform;
    public Transform lake;

    public bool predatorDetected = false;
    public bool threatDetected = false;

    public float seekDistance = 10.0f;
    public float dangerZone = 4.0f;

    [SerializeField]
    static Vector3 randomTarget;

    public enum States
    {
        Flee, Sleep, Search, Drink, Eat, Dead, Wander
    }

    [SerializeField]
    private States currentState;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, perception);
    }

    private void Start()
    {
        currentState = States.Wander;
    }

    private void Update()
    {
        if(health <= 0)
        {
            ChangeState(States.Dead);
        }

        if(currentState != States.Dead)
        {
            timer += Time.deltaTime;
        }
       
        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, perception);
        Collider[] attackedRadius = Physics.OverlapSphere(transform.position, dangerZone);

        predatorDetected = false;
        waterInRange = false;

        if (!cougarIsSneaking)
        {
            foreach (var hitCollider in detectionRadius)
            {
                if (hitCollider.name == predatorTransform.name)
                {
                    predatorDetected = true;
                }
            }
        }

        if(predatorDetected)
        {
            sneakTimer = 0.0f;
            System.Random sneakScore = new System.Random();
            cougarSneak = sneakScore.Next(8);
            cougarIsSneaking = true;
        }

        if (cougarIsSneaking)
        {
            sneakTimer += Time.deltaTime;
        }

        foreach (var hitCollider in attackedRadius)
        {
            if (hitCollider.name == predatorTransform.name && threatDetected == false)
            {
                health -= 10;
                threatDetected = true;
            }
        }

        if (threatDetected)
        {
            timeSinceAttacked += Time.deltaTime;
            if(timeSinceAttacked >= 3)
            {
                timeSinceAttacked = 0;
                threatDetected = false;
            }
        }

        switch (currentState)
        {
            case States.Flee:
                Flee();
                break;
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
            case States.Dead:
                Dead();
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
    }
    
    void Flee()
    {
        cougarIsSneaking = false;
        Vector3 offset = transform.position - predatorTransform.position;
        alphaAgent.velocity = offset.normalized * speed;
        alphaAgent.UpdateMovement();

        if (!predatorDetected)
        {
            fleeTimer += Time.deltaTime;
            if(fleeTimer >= timeSpentFleeing)
            {
                threatDetected = false;
                sneakTimer = 0;
                fleeTimer = 0;
                ChangeState(States.Wander);
            }
        }
    }
    void Sleep()
    {
        if (timer >= 2.0f)
        {
            fatigue -= 2.5f;
        }

        if (fatigue <= 50)
        {
            if (thirst >= 80)
            {
                ChangeState(States.Search);
            }
            else if (hunger >= 80)
            {
                ChangeState(States.Search);
            }
        }
        else if (fatigue <= 0)
        {
            fatigue = 0;
            ChangeState(States.Wander);
        }
    }
    void Search()
    {
        Vector3 offset = lake.position - transform.position;
        alphaAgent.velocity = offset.normalized * speed;
        alphaAgent.UpdateMovement();

        if (sneakTimer > cougarSneak || threatDetected == true)  // the cougar gets a few seconds (random 0-10) to sneak up before it is noticed
        {
            cougarIsSneaking = false;
            ChangeState(States.Flee);
        }
        else
        {
            if (offset.magnitude <= reachedThreshold)
            {
                ChangeState(States.Drink);
            }
        }
    }
    void Drink()
    {
        if (timer >= 2.0f)
        {
            thirst -= 6.0f;
        }

        if (sneakTimer > cougarSneak || threatDetected == true)  // the cougar gets a few seconds (random 0-10) to sneak up before it is noticed
        {
            cougarIsSneaking = false;
            ChangeState(States.Flee);
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

        if (sneakTimer > cougarSneak || threatDetected == true)  // the cougar gets a few seconds (random 0-10) to sneak up before it is noticed
        {
            cougarIsSneaking = false;
            ChangeState(States.Flee);
        }
        else if (hunger <= 0)
        {
            ChangeState(States.Wander);
        }
    }

    void Dead()
    {

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

        alphaAgent.velocity = offset.normalized * speed;
        alphaAgent.UpdateMovement();

        if (sneakTimer > cougarSneak || threatDetected == true)  // the cougar gets a few seconds (random 0-10) to sneak up before it is noticed
        {
            cougarIsSneaking = false;
            ChangeState(States.Flee);
        }
        else if (fatigue >= 50)
        {
            ChangeState(States.Sleep);
        }
        else if (thirst >= 50)
        {
            ChangeState(States.Search);
        }
        else if (hunger >= 50)
        {
            ChangeState(States.Eat);
        }
    }

    void ChangeState(States newState)
    {
        switch (currentState)
        {
            case States.Flee:
                OnFleeExit();
                if(newState == States.Dead)
                {
                    OnDeadEnter();
                }
                else
                {
                    OnWanderEnter();
                }
                break;
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
                if(newState == States.Flee)
                {
                    OnFleeEnter();
                }
                else
                {
                    OnDrinkEnter();
                }
                break;
            case States.Drink:
                OnDrinkExit();
                if (newState == States.Flee)
                {
                    OnFleeEnter();
                }
                else if (newState == States.Eat)
                {
                    OnEatEnter();
                }
                else
                {
                    OnWanderEnter();
                }
                break;
            case States.Eat:
                OnEatExit();
                if (newState == States.Flee)
                {
                    OnFleeEnter();
                }
                else if(newState == States.Search)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnWanderEnter();
                }
                break;
            case States.Wander:
                OnWanderExit();
                if (newState == States.Sleep)
                {
                    OnSleepEnter();
                }
                else if (newState == States.Flee)
                {
                    OnFleeEnter();
                }
                else if (newState == States.Search)
                {
                    OnSearchEnter();
                }
                else
                {
                    OnEatEnter();
                }
                break;
        }
    }

    // Flee
    void OnFleeEnter()
    {
        // anything that happens when flee state starts
        currentState = States.Flee;
    }
    void OnFleeExit()
    {
        // anything that happens when flee state ends
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

    // Dead
    void OnDeadEnter()
    {
        currentState = States.Dead;
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
