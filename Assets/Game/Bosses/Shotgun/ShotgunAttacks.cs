using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    public class ShotgunAttacks : MonoBehaviour, ICanAttack {
        [SerializeField] Projectile projPrefab;
        private GameObject player;
        private int _ammoCount;
        private Projectile bulletInChamber;

        private Vector3 center = new Vector3(0, 2, 0);
        private float speed;
        List<Projectile> bullets = new();

        private int curAttack = -1;

        [SerializeField] private Transform projSpawnPos;
        
        [SerializeField] private GameObject shellPrefab;
        [SerializeField] private List<Transform> shellEjectPositions;
        [SerializeField] private float shellEjectForce = 20;
        
        [SerializeField] private AudioClip shootSFX;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index) {
            curAttack = index;
            switch (index) {
                case 0:
                    AudioSource.PlayClipAtPoint(shootSFX, projSpawnPos.position);
                    return MoveToCenter();
            }

            return 0;
        }

        private float MoveToCenter() {
            speed = Vector3.Distance(center, transform.position) / 50;
            //timer.Set(1, 2);
            return 2.25f;
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case 0:
                    break;
            }
        }
        
        private void FixedUpdate() {
            switch (curAttack) {
                case 0:
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .025f);
                    break;
            }

        }
        
        public void EjectShells() { // this is invoked by animation event
            //create shell projectile in each eject position
            // apply force in direction of forward direction of the eject position
            foreach (Transform ejectPos in shellEjectPositions) {
                GameObject shell = Instantiate(shellPrefab, ejectPos.position, shellPrefab.transform.rotation);
                shell.GetComponent<Rigidbody>().AddForce(ejectPos.forward * shellEjectForce, ForceMode.Impulse);
            }
            
            
        }

        public void ShootArc() {
            
        }
    }

}
