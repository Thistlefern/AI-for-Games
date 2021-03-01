using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AISteeringController : MonoBehaviour
{
    public Agent agent;

    [Header("Steering Settings")]
    public float maxSpeed = 3.0f;
    public float maxForce = 5.0f;
    static float timeToMove = 3.0f;
    static float timer = timeToMove;


    public class SteeringBehavior   // a base from which to build other behaviors
    {
        public virtual Vector3 Steer(AISteeringController controller)
        {
            return Vector3.zero;    // defaults to zero, because if an entity isn't affected by a behavior, they should not move
        }
    }

    public class SeekSteering : SteeringBehavior
    {
        public Transform target;
        public override Vector3 Steer(AISteeringController controller)
        {
            return (target.position - controller.transform.position).normalized * controller.maxSpeed;
        }
    }

    public class FleeSteering : SteeringBehavior
    {
        public Transform target;
        public override Vector3 Steer(AISteeringController controller)
        {
            return (controller.transform.position).normalized * controller.maxSpeed - target.position;
        }
    }

    public class WanderSteering : SteeringBehavior
    {
        public Transform target;

        public override Vector3 Steer(AISteeringController controller)
        {
            if (timer >= timeToMove)
            {
                var randomTarget = (Random.insideUnitSphere * 100) + target.position;
                randomTarget.y = 0;

                Debug.Log(randomTarget);
                timer = 0.0f;

                return randomTarget;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }



    // Add behaviors to this to consider them when calculating steering forces (This happens in Start() section)
    protected List<SteeringBehavior> steerings = new List<SteeringBehavior>();

    // Returns a Vector3 indicating the steering force to apply to our velocity
    protected Vector3 CalculateSteeringForce()
    {
        Vector3 steeringForce = Vector3.zero;
        for (int i = 0; i < steerings.Count; i++)
        {
            steeringForce += steerings[i].Steer(this);  // accumulate steering forces
        }
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);    // truncate to match maxForce value

        return steeringForce;
    }

    // TODO: These are specific to a certain configuration of behaviors and are only here for testing purposes.
    // In a production scenario, you'd likely inherit from AISteeringController
    // and have each derived type have its own parameters for initializing behaviors.
    [SerializeField]
    public Transform wanderTarget;
    public Transform seekTarget;
    //public Transform fleeTarget;
    
    private void Start()
    {
        // add the different steering types to the 'steerings' list
        steerings.Add(new SeekSteering { target = seekTarget });
        // steerings.Add(new FleeSteering { target = fleeTarget });
        steerings.Add(new WanderSteering { target = wanderTarget });

    }
    private void Update()
    {
        timer += Time.deltaTime;
        //wanderTarget = new Vector3(Random.insideUnitSphere.x, 0.0f, Random.insideUnitSphere.z);
        //Debug.Log(wanderTarget);

        Vector3 steeringForce = CalculateSteeringForce();
        // Add steering force to velocity -- clamp its magnitude w/ respect to maxSpeed
        agent.velocity = Vector3.ClampMagnitude(agent.velocity + steeringForce,
                                                maxSpeed);
        agent.UpdateMovement();
    }
}


