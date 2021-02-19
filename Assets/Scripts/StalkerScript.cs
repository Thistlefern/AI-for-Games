using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerScript : MonoBehaviour
{
    public Agent agent;
    public float speed = 3.0f;

    public Transform target;

    private void Update()
    {
        Vector3 offset = target.position - transform.position;

        agent.velocity = offset.normalized * speed;
        agent.UpdateMovement();
    }
}
