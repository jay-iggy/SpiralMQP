using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceMovementComponent : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        MovementComponent movementComponent = other.gameObject.GetComponent<MovementComponent>();
        if (movementComponent != null) {
            movementComponent.moveVelocity = Vector3.Reflect(movementComponent.moveVelocity, other.contacts[0].normal);
            // flip the rotation in the direction of the bounce
            movementComponent.transform.rotation = Quaternion.LookRotation(movementComponent.moveVelocity);
            movementComponent.transform.eulerAngles = new Vector3(0, movementComponent.transform.eulerAngles.y, 0);
        }
    }
}
