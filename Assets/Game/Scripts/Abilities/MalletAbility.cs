using System;
using System.Collections;
using Game.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Abilities {
    public class MalletAbility : AttackAbility {
        [Header("Melee")]
        [SerializeField] GameObject prefabMallet;
        private GameObject mallet;
        [SerializeField] Vector3 malletOffset;
        [SerializeField] float malletCooldown;
        [SerializeField] float malletDuration;
        [SerializeField] float secondsAtPeak = 0;
        [SerializeField] float secondsOnGround = 0;
        private float _malletTimer = 0;
        public float dmg = 10;
        public float knockback = 10;
        private float speed;
        private float increment; // for pos and rot
        private float accumaltedRot = 0;
        private bool holding = false;

        public UnityEvent onHit;

        //audio stuff
        public Sound sfx;

        private void Start() {
            baseDamage = dmg;
            Quaternion startingRotation = prefabMallet.transform.rotation;

            mallet = Instantiate(prefabMallet, this.transform.position + malletOffset, prefabMallet.transform.rotation, this.transform);
            mallet.SetActive(false);
            
            Transform reticle = this.transform.parent.parent.parent.GetChild(2);
            Quaternion toRotation = Quaternion.LookRotation(reticle.position - mallet.transform.position, Vector3.up);
            mallet.transform.eulerAngles = new Vector3(0, toRotation.eulerAngles.y, 0);

            if (mallet.TryGetComponent(out Hitbox hitbox)) {
                BindHitbox(hitbox);
            }

            // mStarCooldown /= CustomStatsManager.instance.customStats.playerAttackSpeed;
            // mStarCooldown *= CustomStatsManager.instance.customStats.playerAttackSpeed;

            increment = 90 / malletDuration;
        }

        void BindHitbox(Hitbox hitbox) {
            hitbox.onHitTarget.AddListener(ProcessAttack);
        }
        private void ProcessAttack(ICanGetHit hurtbox) {
            hurtbox.GetHit(CalculateDamage());
            PlaySound();
            onHit.Invoke();
            
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
            if (_malletTimer > 0) {
                return;
            }

            onAttack.Invoke();
            mallet.transform.position = this.transform.position;
            mallet.SetActive(true);
            holding = true;
        }

        public void Update()
        {
            if (holding)
            {
                Transform reticle = this.transform.parent.parent.parent.GetChild(2);
                Quaternion toRotation = Quaternion.LookRotation(reticle.position - mallet.transform.position, Vector3.up);
                mallet.transform.eulerAngles = new Vector3(0, toRotation.eulerAngles.y, 0);
            }
        }

        public override void AbilityReleased() {
            holding = false;

            _malletTimer = malletDuration;

            StartCoroutine(ResetPunchTimer());
        }
        public override void OnAbilityUnequipped() {
            mallet.SetActive(false);
            mallet.transform.rotation = Quaternion.Euler(0, 0, 0);
            accumaltedRot = 0;
        }
        
        private IEnumerator ResetPunchTimer() {
            while (_malletTimer > 0) {
                _malletTimer -= Time.deltaTime;

                // look to reticle
                Transform reticle = this.transform.parent.parent.parent.GetChild(2);
                Quaternion toRotation = Quaternion.LookRotation(reticle.position - mallet.transform.position, Vector3.up);

                // increment slam down
                float step = increment * Time.deltaTime;
                accumaltedRot += step;
                Debug.Log(accumaltedRot);

                // x: slam, y: reticleLook, z: doesn't change
                mallet.transform.eulerAngles = new Vector3(accumaltedRot, toRotation.eulerAngles.y, mallet.transform.rotation.z);

                yield return null;
            }

            yield return new WaitForSeconds(secondsOnGround);

            mallet.SetActive(false);
            mallet.transform.rotation = Quaternion.Euler(0, 0, 0);
            accumaltedRot = 0;
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