using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{

    void Update()
    {
        WallCollisionDetection();
    }

    //A simulated collision with the boundary walls to avoid the player outside the camera field
    void WallCollisionDetection()
    {
        Vector3 playerObjectPosition = transform.parent.position;
        //Upper border collision
        if (transform.parent.position.y > 2.1f)
            transform.parent.position = new Vector3(playerObjectPosition.x, 2.1f, playerObjectPosition.z);
        //Bottom border collision
        if (transform.parent.position.y < -5f)
            transform.parent.position = new Vector3(playerObjectPosition.x, -5f, playerObjectPosition.z);
        //Left border collision
        if (transform.parent.position.x < -8.3f)
            transform.parent.position = new Vector3(-8.3f, playerObjectPosition.y, playerObjectPosition.z);
        //Right border collision
        if (transform.parent.position.x > 8)
            transform.parent.position = new Vector3(8, playerObjectPosition.y, playerObjectPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        GetComponentInParent<CharacterController>().BodyCollisions(_other);
    }

    private void OnTriggerStay2D(Collider2D _other)
    {
        GetComponentInParent<CharacterController>().BodyCollisions(_other);
    }
}
