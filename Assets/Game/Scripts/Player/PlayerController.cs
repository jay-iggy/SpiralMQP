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

    [Header("Combat Settings")]
    [SerializeField] GameObject reticle;
    private Vector2 _direction = new Vector2();
    private Vector2 _lookDirection = new Vector2(0, 0);
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
        _playerControls.Player.Fire.canceled += OnPrimaryRelease;
        _playerControls.Player.Look.performed += OnLook;
        _playerControls.Player.Dodge.performed += OnSecondary;
        _playerControls.Player.Dodge.canceled += OnSecondaryReleased;
    }
    
    private void FixedUpdate() {
        UpdateMovement();
        UpdateRotation();
    }

    #region Movement
        public void OnMove(InputAction.CallbackContext context) {
            _direction = context.ReadValue<Vector2>();
        }
        private void UpdateMovement() {
            Vector3 targetVelocity = new(_direction.x * movementSpeed, 0, _direction.y * movementSpeed);
            _rb.velocity = Vector3.Lerp(_rb.velocity, targetVelocity, Time.deltaTime * turningRadius);
        }
    #endregion

    #region Rotation and Aiming
        public void OnLook(InputAction.CallbackContext context) {
            _lookDirection += context.ReadValue<Vector2>();
            _lookDirection = Vector2.ClampMagnitude(_lookDirection, maxReticleDistance);
        }
        private void UpdateRotation() {
            Vector3 reticlePos = new(_lookDirection.x / 100, 1, _lookDirection.y / 100);
            reticle.transform.localPosition = reticlePos;
            Quaternion toRotation = Quaternion.LookRotation(reticlePos, Vector3.up);
            playerModel.transform.eulerAngles = new Vector3(0, toRotation.eulerAngles.y-90, 0);
        }
    #endregion
    
    #region Primary Ability
        public void OnPrimary(InputAction.CallbackContext context) {
            primaryAbility.AbilityPressed();
        }
        public void OnPrimaryRelease(InputAction.CallbackContext context) {
            primaryAbility.AbilityReleased();
        }
        public void SetPrimaryAbility(Ability ability) {
            Destroy(primaryAbility); // clear previous ability
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
            Destroy(secondaryAbility); // clear previous ability
            ability.transform.parent = abilityParent;
        
            secondaryAbility = ability;
            secondaryAbility.BindToPlayer(this);
        }
    #endregion
}