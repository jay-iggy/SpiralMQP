using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float turningRadius;
    [SerializeField] GameObject fist;
    private Vector2 direction = new Vector2();
    private Rigidbody rb;

    public PlayerInput playerControls;
    private InputAction move;
    private InputAction fire;

    bool punch = false;

    private void Awake()
    {
        playerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        direction = move.ReadValue<Vector2>();
        
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(direction.x * movementSpeed, 0, direction.y * movementSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * turningRadius);
    }

}
