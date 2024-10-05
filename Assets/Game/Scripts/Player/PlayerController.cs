using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    
    // TODO: velocity is overriden by the movement script, so the player can't be pushed by other sources
    // player rigidbody is getting affected by other things and the rotation is getting fucky
    
    [Header("Movement Settings")]
    public float walkSpeed;
    public float runSpeed;
    [HideInInspector]public float movementSpeed;
    [SerializeField] float turningRadius;
    [SerializeField] GameObject playerModel;
    [HideInInspector]public bool isRunning;
    private Vector3 movementVelocity;
    private Vector3 externalVelocity;
    private Vector3 personalVelocity;
    [SerializeField] private float personalVelocityDampingSpeed = 5;
    [SerializeField] private float externalVelocityDampingSpeed = 5;

    [Header("Combat Settings")]
    [SerializeField] GameObject reticle;
    private Vector2 _direction = new Vector2();
    private Vector2 lookDirection = new Vector2(0, 0);
    [SerializeField] private float maxReticleDistance = 300;

    [Header("Abilities")]
    public Transform abilityParent;
    public Ability primaryAbility;
    public Ability secondaryAbility;
    
    // References
    private Rigidbody _rb;
    private PlayerInput _playerControls; // this isn't a PlayerInput component, its a compiled input action asset named PlayerInput

    private void Awake() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        _rb = GetComponent<Rigidbody>();
        
        CreatePlayerControls();
    }
    void Start(){
        walkSpeed = CustomStatsManager.instance.customStats.playerSpeed;
        HealthComponent healthComponent = GetComponent<HealthComponent>();
        healthComponent.maxHealth = CustomStatsManager.instance.customStats.playerHealth;
        healthComponent.SetHealth(healthComponent.maxHealth);
        
        movementSpeed = walkSpeed;
        
        SetPrimaryAbility(primaryAbility);
        SetSecondaryAbility(secondaryAbility);
    }

    private void OnEnable() {
        _playerControls.Enable();
    }
    private void OnDisable(){
        _playerControls.Disable();
    }

    private void OnDestroy() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CreatePlayerControls() {
        _playerControls = new PlayerInput();
        _playerControls.Player.Move.performed += OnMove;
        _playerControls.Player.Move.canceled += OnMove;
        _playerControls.Player.Fire.performed += OnPrimary;
        _playerControls.Player.Fire.canceled += OnPrimaryReleased;
        _playerControls.Player.Look.performed += OnLook;
        _playerControls.Player.Dodge.performed += OnSecondary;
        _playerControls.Player.Dodge.canceled += OnSecondaryReleased;
    }
    
    private void FixedUpdate() {
        UpdateMovement();
        UpdateRotation();
        
        _rb.velocity = movementVelocity + externalVelocity + personalVelocity;
        
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, Time.deltaTime * externalVelocityDampingSpeed);
        personalVelocity = Vector3.Lerp(personalVelocity, Vector3.zero, Time.deltaTime * personalVelocityDampingSpeed);
    }
    
    public void AddExternalForce(Vector3 velocity) {
        externalVelocity += velocity;
    }
    public void AddPersonalForce(Vector3 force) {
        personalVelocity += force;
    }

    #region Movement
        public void OnMove(InputAction.CallbackContext context) {
            _direction = context.ReadValue<Vector2>();
        }
        private void UpdateMovement() {
            Vector3 targetVelocity = new(_direction.x * movementSpeed, 0, _direction.y * movementSpeed);
            movementVelocity = Vector3.Lerp(_rb.velocity, targetVelocity, Time.deltaTime * turningRadius);
        }
        public Vector2 GetMovementInput() {
            return _playerControls.Player.Move.ReadValue<Vector2>();
        }
    #endregion

    #region Rotation and Aiming
        public void OnLook(InputAction.CallbackContext context) {
            lookDirection += context.ReadValue<Vector2>();
            lookDirection = Vector2.ClampMagnitude(lookDirection, maxReticleDistance);
        }
        private void UpdateRotation() {
            Vector3 reticlePos = new(lookDirection.x / 100, 1, lookDirection.y / 100);
            reticle.transform.position = reticlePos + transform.position;
            Quaternion toRotation = Quaternion.LookRotation(reticlePos, Vector3.up);
            transform.eulerAngles = new Vector3(0, toRotation.eulerAngles.y, 0);
        }
    #endregion
    
    #region Primary Ability
        public void OnPrimary(InputAction.CallbackContext context) {
            primaryAbility.AbilityPressed();
        }
        public void OnPrimaryReleased(InputAction.CallbackContext context) {
            primaryAbility.AbilityReleased();
        }
        public void SetPrimaryAbility(Ability ability) {
            if (ability != primaryAbility) {
                Destroy(primaryAbility); // clear previous ability
            }
            ability.transform.parent = abilityParent;
        
            primaryAbility = ability;
            primaryAbility.BindToPlayer(this);
        }
    #endregion

    #region Secondary Ability
        public void OnSecondary(InputAction.CallbackContext context) {
            secondaryAbility.AbilityPressed();
        }
        public void OnSecondaryReleased(InputAction.CallbackContext context) {
            secondaryAbility.AbilityReleased();
        }
        public void SetSecondaryAbility(Ability ability) {
            if (ability != secondaryAbility) {
                Destroy(secondaryAbility); // clear previous ability
            }
            ability.transform.parent = abilityParent;
        
            secondaryAbility = ability;
            secondaryAbility.BindToPlayer(this);
        }
    #endregion
}