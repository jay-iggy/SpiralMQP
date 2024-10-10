using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(MovementComponent))]
public class PlayerController : MonoBehaviour {
    [Header("Movement Settings")]
    public float walkSpeed;
    [HideInInspector] public float movementSpeed;
    [SerializeField] float movementLerpSpeed = 15;
    [SerializeField] GameObject playerModel;
    [HideInInspector] public MovementComponent movementComponent;

    [Header("Combat Settings")]
    [SerializeField] GameObject reticle;
    [SerializeField] private float maxReticleDistance = 300;
    private Vector2 _inputDirection = new Vector2();
    private Vector2 _cumulativeLookInput = new Vector2(0, 0); // look inputs are in delta amounts, this is the sum of all inputs
    

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
        movementComponent = GetComponent<MovementComponent>();
        
        CreatePlayerControls();
    }
    void Start(){
        // set player stats to custom values
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
        _playerControls.Player.Look.performed += OnLook;
        _playerControls.Player.Fire.performed += OnPrimary;
        _playerControls.Player.Fire.canceled += OnPrimaryReleased;
        _playerControls.Player.Dodge.performed += OnSecondary;
        _playerControls.Player.Dodge.canceled += OnSecondaryReleased;
    }
    
    private void FixedUpdate() {
        UpdateMovement();
        UpdateRotation();
    }

    #region Movement
        public void OnMove(InputAction.CallbackContext context) {
            _inputDirection = context.ReadValue<Vector2>();
        }
        private void UpdateMovement() {
            Vector3 targetVelocity = new(_inputDirection.x * movementSpeed, 0, _inputDirection.y * movementSpeed);
            movementComponent.moveVelocity = Vector3.Lerp(_rb.velocity, targetVelocity, Time.deltaTime * movementLerpSpeed);
        }
        public Vector2 GetMovementInput() {
            return _playerControls.Player.Move.ReadValue<Vector2>();
        }
    #endregion

    #region Rotation and Aiming
        public void OnLook(InputAction.CallbackContext context) {
            _cumulativeLookInput += context.ReadValue<Vector2>();
            _cumulativeLookInput = Vector2.ClampMagnitude(_cumulativeLookInput, maxReticleDistance);
        }
        private void UpdateRotation() {
            Vector3 reticlePos = new(_cumulativeLookInput.x / 100, 1, _cumulativeLookInput.y / 100);
            reticle.transform.position = reticlePos + transform.position;
            // rotate player object to face reticle
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