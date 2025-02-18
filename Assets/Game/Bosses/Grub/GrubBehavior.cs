using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrubBehavior : MonoBehaviour
{
    private MovementComponent _movementComponent;
    [SerializeField] private float speed = 5;
    [SerializeField] private float moveDelay = 0.25f;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float moveDistance = 3;
    

    private void Awake() {
        _movementComponent = GetComponent<MovementComponent>();
    }

    private void Start() {
        StartCoroutine(Behavior());
    }


    private IEnumerator Behavior() {
        print("Grub Behavior started");
        while (true) {
            Vector3 targetPosition = (BossRoom.GetRandomPositionInRoom(0) - transform.position).normalized * Random.Range(1, moveDistance);
            
            
            yield return MoveToPosition(targetPosition);
            yield return new WaitForSeconds(moveDelay);
        }
    }
    
    private IEnumerator MoveToPosition(Vector3 targetPosition) {
        while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), targetPosition) > 1f) {
            _movementComponent.moveVelocity = (targetPosition - transform.position).normalized * speed;
            // rotate along y axis to face the move velocity direction
            Vector3 targetDir = _movementComponent.moveVelocity;
            float step = Time.deltaTime * rotationSpeed;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            yield return null;
        }
        _movementComponent.moveVelocity = Vector3.zero;
    }
}
