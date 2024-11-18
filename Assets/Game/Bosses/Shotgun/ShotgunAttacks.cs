using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    public class ShotgunAttacks : MonoBehaviour, ICanAttack {
        [SerializeField] GameObject projPrefab;
        private GameObject player;
        private int _ammoCount;
        private GameObject bulletInChamber; // we can have this be type Projectile

        private Vector3 center = new Vector3(0, 2, 0);
        private float speed;
        GameObject[] bullets = new GameObject[12]; // why not just use a list?

        private int curAttack = -1;

        [SerializeField] private Transform projSpawnPos;
        
        [SerializeField] private GameObject shellPrefab;
        [SerializeField] private List<Transform> shellEjectPositions;

        //audio
        public AudioManager audioCon;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index) {
            curAttack = index;
            switch (index) {
                case 2:
                    audioCon.PlaySFX("shoot_demo");
                    return MoveToCenter();
            }

            return 0;
        }

        private float MoveToCenter() {
            speed = Vector3.Distance(center, transform.position) / 50;
            //timer.Set(1, 2);
            return 2.25f;
        }

        private void ShootCirclePattern() {
            curAttack = 3;
            for(int i = 0; i < 12; i++) {
                bullets[i] = Instantiate(projPrefab);
            }
            BulletPatterns.CreateCircle(bullets, transform.position, 1);
            //timer.Set(.25f, 3);
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case 2:
                    ShootCirclePattern();
                    break;
                case 3:
                    BulletPatterns.MoveTowards(bullets, transform.position, -8);
                    bullets = new GameObject[12];
                    break;
            }
        }
        
        private void FixedUpdate() {
            switch (curAttack) {
                case 1:
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .025f);
                    break;
                case 2:
                    transform.position = Vector3.MoveTowards(transform.position, center, speed);
                    break;
            }

        }
        
        public void EjectShells() {
            // we can have a shell prefab and instantiate it here
            
            
        }
    }

}
