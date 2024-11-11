using System;
using System.Collections;
using Game.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class MalletAbility : Ability {
        [Header("Melee")]
        [SerializeField] GameObject prefabMallet;
        private GameObject mallet;
        [SerializeField] Vector3 malletOffset;
        [SerializeField] float malletCooldown;
        [SerializeField] float malletDuration;
        [SerializeField] float secondsOnGround = 0;
        private float _malletTimer = 0;
        public float dmg = 2;
        public float knockback = 10;
        private float speed;
        private float increment; // for pos and rot
        private float accumaltedRot = 0;
        
        private void Start() {

            Quaternion startingRotation = prefabMallet.transform.rotation;

            mallet = Instantiate(prefabMallet, this.transform.position + malletOffset, prefabMallet.transform.rotation, this.transform);

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
            hurtbox.GetHit(dmg);
            
            // knockback the target
            if(hurtbox is MonoBehaviour target) {
                Vector3 direction = target.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                target.GetComponent<MovementComponent>().AddExternalVelocity(direction * knockback);
            }
        }
        
        public override void AbilityPressed() {
            if (_malletTimer > 0) {
                return;
            }
            mallet.transform.position = this.transform.position;
            mallet.SetActive(true);
            _malletTimer = malletDuration;
            
            StartCoroutine(ResetPunchTimer());
        }
        public override void AbilityReleased() {
            // nothing to do here
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
    }
}