using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // Refers to the rigidbody that will be driven
    // (assign this in the editor)
    [SerializeField]
    public Rigidbody rbody;
    public Collider myCollider;
    
    // Defines the change in the player's position over time
    public virtual Vector3 velocity { get; set; }
    
    // Integrate movement and update the player's position
    public virtual void UpdateMovement()
    {
        // Moves the rigidbody to the new position
        //
        // The new position is equal to pos + vel * dT
        rbody.MovePosition(rbody.position + velocity * Time.deltaTime);
    }
}