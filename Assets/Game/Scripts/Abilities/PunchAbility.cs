using System;
using System.Collections;
using Game.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Abilities
{

    public class PunchAbility : AttackAbility
    {

        [Header("Melee")]
        [SerializeField] float punchCooldown = .25f;
        [SerializeField] float punchDuration = .5f;
        private float _punchTimer = 0;
        public float dmg = 3;
        public float knockback = 5;
        [SerializeField] private float invinciblityDurationAfterHit = 0.1f;
        
        
        [Header("Combo")]
        public float comboDamage = 5;
        public int maxCombo = 3;
        public float comboDuration = 1;
        private int _comboCounter = 0;
        private float _comboTimer = 0;
        private Coroutine _comboCoroutine;
        [SerializeField] float delayBetweenCombos = 0.5f;
        private float _comboDelayTimer = 0;
        
        // heavy attack combo:
        // first attack: charge up to lunge
        // second attack: horizontal swipe
        // third attack: overhead slam
        
        // other mode: flurry of punches
        // quicker, but takes more consecutive hits to crit
        
        [Header("GFX")]
        [SerializeField] GameObject fist;
        [SerializeField] Transform target;
        [SerializeField] Transform outStretch;
        
        [Header("VFX")]
        [SerializeField] GameObject hitEffect;
        [SerializeField] GameObject critEffect;
        
        //audio
        [SerializeField] Sound sfx;

        public override void OnAbilityEquipped() {
            _player.GetReticle().enabled = false;
        }

        private void Start()
        {
            baseDamage = dmg;

            if (fist.TryGetComponent(out Hitbox hitbox))
            {
                BindHitbox(hitbox);
            }

            punchCooldown /= CustomStatsManager.instance.customStats.playerAttackSpeed;
            punchDuration *= CustomStatsManager.instance.customStats.playerAttackSpeed;
        }

        void BindHitbox(Hitbox hitbox)
        {
            hitbox.onHitTarget.AddListener(ProcessAttack);
        }
        private void ProcessAttack(ICanGetHit hurtbox) {
            float dmg = CalculateDamage();
            bool isCrit = false;
            
            Math.Clamp(_comboCounter++, 0, maxCombo);
            
            if (_comboCounter == maxCombo) {
                dmg *= 2;
                isCrit = true;
                _comboCounter = 0;
                _comboTimer = 0;
                if(_comboCoroutine != null) {
                    StopCoroutine(_comboCoroutine);
                }
                Instantiate(critEffect, transform.position, Quaternion.identity);
                _comboDelayTimer = Time.time + delayBetweenCombos;
            }
            else {
                _comboTimer = comboDuration;
                if (_comboCoroutine == null) {
                    _comboCoroutine = StartCoroutine(UpdateCombo());
                }
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            
            hurtbox.GetHit(dmg, false, isCrit);
            
            
            _player.GetHealthComponent().BecomeInvincible(invinciblityDurationAfterHit);
            
            PlaySound();

            // knockback the target
            if (hurtbox is MonoBehaviour target)
            {
                Vector3 direction = target.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                MovementComponent movementComponent = target.GetComponent<MovementComponent>();
                if (movementComponent != null)
                {
                    movementComponent.AddExternalVelocity(direction * knockback);
                }
            }
        }

        bool isHeld = false;
        float heldTimer = 0;
        [SerializeField]private float maxHeldTime = 0.5f;
        [SerializeField] private AnimationCurve heldTimeCurve;
        public override void AbilityPressed()
        {
            if (_comboDelayTimer > Time.time) {
                return;
            }
            
            if (_punchTimer > 0) {
                return;
            }
            isHeld = true;
            if (_comboCounter > 0) {
                StartPunch();
            }
        }

        private void StartPunch() {
            onAttack.Invoke();
            fist.SetActive(true);
            _punchTimer = punchCooldown + punchDuration;
            StartCoroutine(ResetPunchTimer());
        }

        public override void AbilityReleased()
        {
            if (_comboCounter < 1) {
                if (isHeld) {
                    _player.movementComponent.AddPersonalVelocity(_player.transform.forward*heldTimeCurve.Evaluate(heldTimer/maxHeldTime));
                    StartCoroutine(StartPunchAfterDelay());
                }
            }
           
            isHeld = false;
            heldTimer = 0;
        }

        private IEnumerator StartPunchAfterDelay() {
            yield return heldTimer;
            StartPunch();
        }


        void Update() {
            if (isHeld) {
                heldTimer = Mathf.Clamp(heldTimer + Time.deltaTime, 0 ,maxHeldTime);
            }
        }

        public override void OnAbilityUnequipped()
        {
            fist.SetActive(false);
            _punchTimer = 0;
        }


        private IEnumerator ResetPunchTimer()
        {
            // could just do a yield return new WaitForSeconds
            // though the other code is set up to support this way

            while (_punchTimer > 0)
            {
                float normalizedTime = 1 - (_punchTimer / punchCooldown + punchDuration);
                target.position = Vector3.Lerp(target.position, outStretch.position, normalizedTime);

                _punchTimer -= Time.deltaTime;
                yield return null;
            }
            fist.SetActive(false);
        }


        private void PlaySound()
        {
            if (sfx != null) sfx.PlaySound();
        }


        private IEnumerator UpdateCombo() {
            while(_comboCounter>0) {
                _comboTimer -= Time.deltaTime;
                if(_comboTimer <= 0) {
                    _comboCounter--;
                    if(_comboCounter>0) {
                        _comboTimer = comboDuration;
                    }
                }
                yield return null;
            }
        }
    }
    
}