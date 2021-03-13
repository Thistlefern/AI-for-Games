using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerScript : MonoBehaviour
{
    public Agent deerAgent;
    public float speed = 8.0f;
    public float health = 40.0f;
    public float fatigue = 0.0f;
    public float thirst = 0.0f;
    public float hunger = 40.0f;
    public float perception = 20.0f;

    public float reachedThreshold = 0.5f;

    public float timer = 0.0f;

    public Transform predatorTransform;
    public Transform alphaDeer;
    public Transform lake;

    public bool predatorDetected = false;

    public float seekDistance = 10.0f;

    static Vector3 randomTarget;

    public enum States
    {
        Flee, Sleep, Search, Drink, Eat, Wander
    }

    [SerializeField]
    private States currentState;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, perception);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2.0f)
        {
            fatigue += 0.5f;
            hunger++;
            thirst++;
            timer = 0.0f;
        }

        // TODO add if deer is too far from alpha, go to alpha

        Collider[] detectionRadius = Physics.OverlapSphere(transform.position, perception);

        // TODO fix so other deer don't trigger predator check
        //foreach (var hitCollider in detectionRadius)
        //{
        //    if (hitCollider.name != name)
        //    {
        //        predatorDetected = true;
        //    }
        //}

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
            default:
                Wander();
                break;
        }

        predatorDetected = false;
    }

    void Flee()
    {
        // TODO flee

        // TODO if predator transform is not in perception range, wander
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
        // TODO search for water
        // TODO implement stopping search when water is reached
        // TODO implement flee if predator gets too close
    }
    void Drink()
    {
        if (timer >= 2.0f)
        {
            thirst -= 6.0f;
        }

        // TODO implement flee if predator gets too close
        // else
        if (thirst <= 0)
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

        // TODO implement flee if predator gets too close
        // else
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
    void Wander()
    {
        // TODO wander
        // TODO implement flee if predator gets too close
        // else
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
            case States.Flee:
                OnFleeExit();
                OnWanderEnter();
                break;
            case States.Sleep:
                OnSleepExit();
                if (newState == States.Search)
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
                if (newState == States.Flee)
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
                else if (newState == States.Search)
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
                if (newState == States.Flee)
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
