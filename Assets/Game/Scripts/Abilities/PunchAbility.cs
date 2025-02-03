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
        [SerializeField] GameObject fist;
        [SerializeField] Transform target;
        [SerializeField] Transform outStretch;
        [SerializeField] float punchCooldown = .25f;
        [SerializeField] float punchDuration = .5f;
        private float _punchTimer = 0;
        private float initialPunchTimer;
        public float dmg = 3;
        public float knockback = 5;
        [SerializeField] private Collider magnetismTrigger;

        //audio
        [SerializeField] Sound sfx;

        private void Start()
        {
            baseDamage = dmg;

            if (fist.TryGetComponent(out Hitbox hitbox))
            {
                BindHitbox(hitbox);
            }

            punchCooldown /= CustomStatsManager.instance.customStats.playerAttackSpeed;
            punchDuration *= CustomStatsManager.instance.customStats.playerAttackSpeed;

            initialPunchTimer = punchCooldown + punchDuration;
        }

        void BindHitbox(Hitbox hitbox)
        {
            hitbox.onHitTarget.AddListener(ProcessAttack);
        }
        private void ProcessAttack(ICanGetHit hurtbox)
        {
            hurtbox.GetHit(CalculateDamage());

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


        public override void AbilityPressed()
        {
            if (_punchTimer > 0)
            {
                return;
            }
            onAttack.Invoke();
            fist.SetActive(true);
            _punchTimer = punchCooldown + punchDuration;
            magnetismTrigger.enabled = true;


            StartCoroutine(ResetPunchTimer());
        }

        public override void AbilityReleased()
        {
            // nothing to do here
        }

        public override void OnAbilityUnequipped()
        {
            fist.SetActive(false);
            magnetismTrigger.enabled = false;
            _punchTimer = 0;
        }


        private IEnumerator ResetPunchTimer()
        {
            // could just do a yield return new WaitForSeconds
            // though the other code is set up to support this way

            while (_punchTimer > 0)
            {
                float normalizedTime = 1 - (_punchTimer / initialPunchTimer);
                target.position = Vector3.Lerp(target.position, outStretch.position, normalizedTime);

                _punchTimer -= Time.deltaTime;
                yield return null;
            }
            fist.SetActive(false);
            magnetismTrigger.enabled = false;
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(TagManager.Enemy))
            { // TODO: make this based off hitbox tagsToHit
                Vector3 direction = other.transform.position - transform.position;
                direction.y = 0;
                _player.movementComponent.AddPersonalVelocity(direction * 1);
            }
            PlaySound();
        }
        private void PlaySound()
        {
            if (sfx != null) sfx.PlaySound();
        }
    }
}