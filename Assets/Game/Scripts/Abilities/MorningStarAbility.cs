using System;
using System.Collections;
using Game.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class MorningStarAbility : AttackAbility {
        [Header("Melee")]
        [SerializeField] GameObject prefabMStar;
        private GameObject mStar;
        [SerializeField] Vector3 mStarOffset;
        [SerializeField] float mStarCooldown;
        [SerializeField] float mStarDuration;
        private float _mStarTimer = 0;
        public float dmg = 2;
        public float knockback = 10;
        private float speed;

        public Sound sfx;

        private void Start() {
            baseDamage = dmg;
            Quaternion startingRotation = prefabMStar.transform.rotation;

            mStar = Instantiate(prefabMStar, this.transform.position + mStarOffset, prefabMStar.transform.rotation, this.transform);

            Transform reticle = this.transform.parent.parent.parent.GetChild(2);
            Quaternion toRotation = Quaternion.LookRotation(reticle.position - mStar.transform.position, Vector3.up);
            mStar.transform.eulerAngles = new Vector3(90, toRotation.eulerAngles.y, 0);

            if (mStar.TryGetComponent(out Hitbox hitbox)) {
                BindHitbox(hitbox);
            }
            mStar.SetActive(false);

            // mStarCooldown /= CustomStatsManager.instance.customStats.playerAttackSpeed;
            // mStarCooldown *= CustomStatsManager.instance.customStats.playerAttackSpeed;
        }
        private void Update()
        {
            Transform reticle = this.transform.parent.parent.parent.GetChild(2);
            Quaternion toRotation = Quaternion.LookRotation(reticle.position - mStar.transform.position, Vector3.up);
            mStar.transform.eulerAngles = new Vector3(90, toRotation.eulerAngles.y, 0);
        }

        void BindHitbox(Hitbox hitbox) {
            hitbox.onHitTarget.AddListener(ProcessAttack);
        }
        private void ProcessAttack(ICanGetHit hurtbox) {
            hurtbox.GetHit(CalculateDamage());
            PlaySound();
            
            // knockback the target
            if(hurtbox is MonoBehaviour target) {
                Vector3 direction = target.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                MovementComponent movementComponent = target.GetComponent<MovementComponent>();
                if(movementComponent != null) {
                    movementComponent.AddExternalVelocity(direction * knockback);
                }
            }
        }

        public override void AbilityPressed() {
            if (_mStarTimer > 0) {
                return;
            }
            onAttack.Invoke();
            mStar.transform.position = this.transform.position;
            mStar.SetActive(true);
            _mStarTimer = mStarCooldown + mStarDuration;
            
            StartCoroutine(ResetPunchTimer());
        }
        public override void AbilityReleased() {
            // nothing to do here
        }

        public override void OnAbilityUnequipped() {
            _mStarTimer = 0;
            mStar.SetActive(false);
        }
        
        private IEnumerator ResetPunchTimer() {
            while (_mStarTimer > 0) {
                _mStarTimer -= Time.deltaTime;
                yield return null;
            }
            mStar.SetActive(false);
        }
        
        void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag(TagManager.Enemy)) { // TODO: make this based off hitbox tagsToHit
                Vector3 direction = other.transform.position - transform.position;
                direction.y = 0;
                _player.movementComponent.AddPersonalVelocity(direction * 1);
            }
        }
        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}