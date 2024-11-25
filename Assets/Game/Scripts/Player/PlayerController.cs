using System;
using Game.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementComponent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject playerModel;

    private bool _canMove = true;
    private bool _canLook = true;
    private bool _canAttack = true;

    [Header("Movement Settings")]
    public float walkSpeed;
    [HideInInspector] public float movementSpeed;
    [SerializeField] float movementLerpSpeed = 15;
    [HideInInspector] public MovementComponent movementComponent;
    private Vector2 _movementInput = new Vector2();

    [Header("Look Settings")]
    [SerializeField] GameObject reticle;
    [SerializeField] private float maxReticleDistance = 300;
    public Vector2 _cumulativeLookInput = new Vector2(0, 0); // look inputs are in delta amounts, this is the sum of all inputs


    [Header("Abilities")]
    public Transform abilityParent;
    public Ability primaryAbility;
    public Ability offhandAbility;
    public Ability secondaryAbility;
    public Ability punchAbilityPrefab;

    // References
    private Rigidbody _rb;
    private SpiralPlayerControls _playerControls;
    private HealthComponent healthComponent;


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        _rb = GetComponent<Rigidbody>();
        movementComponent = GetComponent<MovementComponent>();

        CreatePlayerControls();
    }
    void Start()
    {
        // set player stats to custom values
        walkSpeed = CustomStatsManager.instance.customStats.playerSpeed;
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.SetMaxHealth(CustomStatsManager.instance.customStats.playerHealth);

        movementSpeed = walkSpeed;

        VerifyAbilities();
        if (secondaryAbility != null)
        {
            SetSecondaryAbility(secondaryAbility);
        }
    }

    public HealthComponent GetHealthComponent()
    {
        return healthComponent;
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        CombatManager.instance.onPlayerLose.AddListener(Die);
    }
    private void OnDisable()
    {
        _playerControls.Disable();
        CombatManager.instance.onPlayerLose.RemoveListener(Die);
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Die()
    {
        _canMove = false;
        _canLook = false;
        _canAttack = false;
    }

    private void CreatePlayerControls()
    {
        _playerControls = new();
        _playerControls.Player.Move.performed += OnMove;
        _playerControls.Player.Move.canceled += OnMove;
        _playerControls.Player.Look.performed += OnLook;
        _playerControls.Player.Fire.performed += OnPrimary;
        _playerControls.Player.Fire.canceled += OnPrimaryReleased;
        _playerControls.Player.Dodge.performed += OnSecondary;
        _playerControls.Player.Dodge.canceled += OnSecondaryReleased;
        _playerControls.Player.SwapAbility.performed += SwapAbility;
        _playerControls.Player.SwapAbility.canceled -= SwapAbility;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }

    #region Movement
    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }
    private void UpdateMovement()
    {
        Vector3 targetVelocity = new(_movementInput.x * movementSpeed, 0, _movementInput.y * movementSpeed);
        if (!_canMove) targetVelocity = Vector3.zero;
        movementComponent.moveVelocity = Vector3.Lerp(_rb.velocity, targetVelocity, Time.deltaTime * movementLerpSpeed);
    }
    public Vector2 GetMovementInput()
    {
        return _playerControls.Player.Move.ReadValue<Vector2>();
    }
    #endregion

    #region Rotation and Aiming
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!_canLook)
        {
            return;
        }
        _cumulativeLookInput += context.ReadValue<Vector2>();
        _cumulativeLookInput = Vector2.ClampMagnitude(_cumulativeLookInput, maxReticleDistance);
    }
    private void UpdateRotation()
    {
        Vector3 reticlePos = new(_cumulativeLookInput.x / 100, 1, _cumulativeLookInput.y / 100);
        reticle.transform.position = reticlePos + transform.position;
        // rotate player object to face reticle
        Quaternion toRotation = Quaternion.LookRotation(reticlePos, Vector3.up);
        transform.eulerAngles = new Vector3(0, toRotation.eulerAngles.y, 0);
    }
    #endregion

    #region Primary Ability
    public void OnPrimary(InputAction.CallbackContext context)
    {
        if (!_canAttack)
        {
            return;
        }
        primaryAbility.AbilityPressed();
    }
    public void OnPrimaryReleased(InputAction.CallbackContext context)
    {
        primaryAbility.AbilityReleased();

    }
    public void SetPrimaryAbility(Ability ability)
    {
        if (primaryAbility != null)
        {
            if (primaryAbility.GetType() != punchAbilityPrefab.GetType())
            {
                if (offhandAbility == null || offhandAbility.GetType() == punchAbilityPrefab.GetType())
                {
                    primaryAbility.OnAbilityUnequipped();
                    ChangeOffhandAbility(primaryAbility);
                    ChangePrimaryAbility(ability);
                    return;
                }
            }
            else
            {
                Destroy(primaryAbility.gameObject);
            }
        }
        if (ability != primaryAbility)
        {
            Destroy(primaryAbility.gameObject); // clear previous ability
        }
        ChangePrimaryAbility(ability);
    }
    private void ChangePrimaryAbility(Ability ability)
    {
        ability.transform.parent = abilityParent;
        primaryAbility = ability;
        primaryAbility.BindToPlayer(this);
        primaryAbility.gameObject.SetActive(true);
    }

    private void ChangeOffhandAbility(Ability ability)
    {
        ability.transform.parent = abilityParent;
        offhandAbility = ability;
        offhandAbility.BindToPlayer(this);
        offhandAbility.gameObject.SetActive(false);
    }

    #endregion

    #region Secondary Ability
    public void OnSecondary(InputAction.CallbackContext context)
    {
        if (!_canAttack)
        {
            return;
        }

        if (secondaryAbility != null)
            secondaryAbility.AbilityPressed();
    }
    public void OnSecondaryReleased(InputAction.CallbackContext context)
    {
        if (secondaryAbility != null)
            secondaryAbility.AbilityReleased();
    }
    public void SetSecondaryAbility(Ability ability)
    {
        if (ability != secondaryAbility)
        {
            Destroy(secondaryAbility.gameObject); // clear previous ability
        }
        ability.transform.parent = abilityParent;

        secondaryAbility = ability;
        secondaryAbility.BindToPlayer(this);
    }
    #endregion

    private void SwapAbility(InputAction.CallbackContext context)
    {
        if (primaryAbility != null && offhandAbility != null)
        {
            primaryAbility.OnAbilityUnequipped();
            Ability temp = primaryAbility;
            ChangePrimaryAbility(offhandAbility);
            ChangeOffhandAbility(temp);
        }
    }

    private void VerifyAbilities()
    {
        if (primaryAbility == null)
        {
            Ability ability = Instantiate(punchAbilityPrefab, abilityParent);
            ability.transform.rotation = transform.rotation;
            ChangePrimaryAbility(ability);
        }
        if (offhandAbility == null)
        {
            Ability ability = Instantiate(punchAbilityPrefab, abilityParent);
            ability.transform.rotation = transform.rotation;
            ChangeOffhandAbility(ability);
        }
    }
}