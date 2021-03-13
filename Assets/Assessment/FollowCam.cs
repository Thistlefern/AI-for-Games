using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform followPlayer;

    // offset from the player's position
    private Vector3 followOffset;

    // Start is called before the first frame update
    void Start()
    {
        followOffset = transform.position - followPlayer.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followPlayer.position + followOffset;
    }
}
