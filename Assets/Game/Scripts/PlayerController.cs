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
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject playerModel;
    private Vector2 direction = new Vector2();
    private Vector2 lookDirection = new Vector2(0, 0);
    private Rigidbody rb;

    public PlayerInput playerControls;
    private InputAction move;
    private InputAction fire;
    private InputAction turn;

    [SerializeField] float punchCooldown;
    [SerializeField] float punchDuration;
    private float punchTimer;

    private void Awake()
    {
        Cursor.visible = false;
        playerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();

        turn = playerControls.Player.Look;
        turn.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        turn.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        direction = move.ReadValue<Vector2>();
        lookDirection += turn.ReadValue<Vector2>();
        lookDirection = Vector2.ClampMagnitude(lookDirection, 300);

        if(punchTimer <= 0 && fire.ReadValue<float>() > .5f)
        {
            punchTimer = punchCooldown;
            fist.SetActive(true);
        }
        if(punchTimer > 0)
        {
            punchTimer -= Time.deltaTime;
            if(punchTimer < punchCooldown - punchDuration)
            {
                fist.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(direction.x * movementSpeed, 0, direction.y * movementSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * turningRadius);

        Vector3 reticlePos = new Vector3(lookDirection.x / 100, 1, lookDirection.y / 100);
        reticle.transform.localPosition = reticlePos;
        Quaternion toRotation = Quaternion.LookRotation(reticlePos, Vector3.up);
        playerModel.transform.eulerAngles = new Vector3(0, toRotation.eulerAngles.y-90, 0);

        //transform.eulerAngles = new Vector3(0, Vector3.Angle(new Vector3(1, reticle.transform.localPosition.y, 0), reticlePos), 0);
    }

}
