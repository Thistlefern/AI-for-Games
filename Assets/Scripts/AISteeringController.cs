﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISteeringController : MonoBehaviour
{
    public Agent agent;

    [Header("Steering Settings")]
    public float maxSpeed = 3.0f;
    public float maxForce = 5.0f;

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

    // Add behaviors to this to consider them when calculating steering forces
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
    // and have each derived type have its own parameters for initializing
    // behaviors.
    public Transform seekTarget;
    private void Start()
    {
        steerings.Add(new SeekSteering { target = seekTarget });
    }
    private void Update()
    {
        Vector3 steeringForce = CalculateSteeringForce();
        // Add steering force to velocity -- clamp its magnitude w/ respect
        // to maxSpeed
        agent.velocity = Vector3.ClampMagnitude(agent.velocity + steeringForce,
                                                maxSpeed);
        agent.UpdateMovement();
    }
}


