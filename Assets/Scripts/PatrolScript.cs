using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolScript : MonoBehaviour
{
    public Agent agent;
    public float speed = 3.0f;

    public Transform[] waypoints;
    private int currentWaypointID = 0;

    public float reachedThreshold = 0.5f;

    public float waitInterval = 2.0f;      // amount of time to wait at each patrol waypoint in seconds
    public float waitTimer = 2.0f;

    private void Update()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer < waitInterval)
        {
            return;
        }
        else
        {

            Vector3 offset = waypoints[currentWaypointID].position - transform.position;

            agent.velocity = offset.normalized * speed;

            agent.UpdateMovement();

            if(offset.magnitude <= reachedThreshold)    // checks if patrol has reached waypoint
            {
                waitTimer = 0;              // set timer to zero to begin counting time at waypoint
                currentWaypointID++;        // if waypoint was reached, target waypoint becomes next waypoint
                if(currentWaypointID >= waypoints.Length)   // loop back to the beginning after reaching the last waypoint
                {
                    currentWaypointID = 0;
                }
            }
        }
    }
}
