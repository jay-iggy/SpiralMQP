using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementComponent : MonoBehaviour {
    public Vector3 moveVelocity;
    private Vector3 externalVelocity;
    private Vector3 personalVelocity;
    [SerializeField] private float externalVelocityDamping = 5;
    [SerializeField] private float personalVelocityDamping = 5;
    
    private Rigidbody _rb;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    void Update() {
        _rb.velocity = moveVelocity + externalVelocity + personalVelocity;
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, externalVelocityDamping * Time.deltaTime);
        personalVelocity = Vector3.Lerp(personalVelocity, Vector3.zero, personalVelocityDamping * Time.deltaTime);
    }
    
    public void AddExternalVelocity(Vector3 velocity) {
        externalVelocity += velocity;
    }
    public void AddPersonalVelocity(Vector3 velocity) {
        personalVelocity += velocity;
    }
}
