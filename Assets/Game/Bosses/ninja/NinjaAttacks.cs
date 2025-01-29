using System;
using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class NinjaAttacks : MonoBehaviour, ICanAttack
    {
        private const int DONE_POPPING_OUT_SINGLE = 0;
        private const int DONE_POPPING_OUT_SPREAD = 1;
        private const int KILL_DECOYS = 2;
        
        [Tooltip("+x, -x, +z, -z")]
        private float[] wallPositions = new float[4];
        [SerializeField] float startDistanceBehindWall;
        [SerializeField] float popOutSpeed;
        private float minDistanceFromEdge = 2;
        int prevWall = -1;

        private Timer timer;
        private MovementComponent movement;
        private NinjaDecoy nd;
        [SerializeField] GameObject bullet;
        [SerializeField] GameObject smoke;
        [SerializeField] GameObject decoy;
        private GameObject[] decoys = new GameObject[4];
        private GameObject player;


        private Vector3 gun;

        public int GetAttackCount() { return 6; }

        public float Attack(int index)
        {
            /* 1/6 chance of decoy, 5/6 chance of pop out of wall
             * 3/6 chance of shoot single bullet, 2/6 chance of shoot bullet spread
             */
            switch (index)
            {
                case 0:
                    return SpawnDecoys();
                case 1:
                    return PopOut(DONE_POPPING_OUT_SINGLE);
                case 2:
                    return PopOut(DONE_POPPING_OUT_SINGLE);
                case 3:
                    return PopOut(DONE_POPPING_OUT_SINGLE);
                case 4:
                    return PopOut(DONE_POPPING_OUT_SPREAD);
                case 5:
                    return PopOut(DONE_POPPING_OUT_SPREAD);
            }
            return 0;
        }

        private float SpawnDecoys()
        {
            GameObject quickSmoke = Instantiate(smoke, transform.position, Quaternion.identity);
            quickSmoke.GetComponent<FadeAway>().setSeconds(.25f);

            nd.canCollide = true;
            nd.setRandomPosition(wallPositions, minDistanceFromEdge, transform.position.y);

            for(int i = 0; i<decoys.Length; i++)
            {
                decoys[i] = Instantiate(decoy);
                decoys[i].GetComponent<NinjaDecoy>().setRandomPosition(wallPositions, minDistanceFromEdge, transform.position.y);
            }
            timer.Set(5, KILL_DECOYS);
            LayerMask Decoy = LayerMask.GetMask("Decoy");
            GetComponent<BoxCollider>().excludeLayers = Decoy;
            return 5;
        }

        private void PopOutSingle()
        {
            setGunPoint();
            GameObject singleBullet = Instantiate(bullet, gun, Quaternion.identity);
            BulletPatterns.MoveTowards(singleBullet, player.transform.position, 8);
        }

        private void PopOutSpread()
        {
            GameObject[] bulletSpread = new GameObject[12];
            for(int i = 0; i<bulletSpread.Length; i++)
            {
                bulletSpread[i] = Instantiate(bullet);
            }
            BulletPatterns.CreateCircle(bulletSpread, transform.position, .05f);
            BulletPatterns.MoveTowards(bulletSpread, transform.position, -7);
        }

        private float PopOut(int whatToDoAfter)
        {
            int whichWall = prevWall;
            while(whichWall == prevWall)
            {
                whichWall = Random.Range(0, 4);
            }
            prevWall = whichWall;
            float zPos;
            float xPos;
            Vector3 popOutVelocity;
            if (whichWall <= 1) //x
            {               
                zPos = Random.Range(wallPositions[3] + minDistanceFromEdge, wallPositions[2] - minDistanceFromEdge);    
                if (whichWall == 0) //positive x
                {
                    xPos = wallPositions[0] + startDistanceBehindWall;
                    popOutVelocity = new Vector3(-popOutSpeed, 0, 0);                   
                }
                else //negative x
                {
                    xPos = wallPositions[1] - startDistanceBehindWall;
                    popOutVelocity = new Vector3(popOutSpeed, 0, 0);
                }
            }
            else //y
            {
                xPos = Random.Range(wallPositions[1] + minDistanceFromEdge, wallPositions[0] - minDistanceFromEdge);
                if (whichWall == 2) //positive z
                {
                    zPos = wallPositions[2] + startDistanceBehindWall;
                    popOutVelocity = new Vector3(0, 0, -popOutSpeed);
                }
                else //negative z
                {
                    zPos = wallPositions[3] - startDistanceBehindWall;
                    popOutVelocity = new Vector3(0, 0, popOutSpeed);
                }
            }
            transform.position = new Vector3(xPos, transform.position.y, zPos);
            movement.moveVelocity = popOutVelocity;
            timer.Set(.2f, whatToDoAfter);

            if (whatToDoAfter == DONE_POPPING_OUT_SINGLE) return .75f;
            else if (whatToDoAfter == DONE_POPPING_OUT_SPREAD) return 1.75f;
            else return 0;
        }
        
        void Start()
        {
            timer = GetComponent<Timer>();
            movement = GetComponent<MovementComponent>();
            nd = GetComponent<NinjaDecoy>();
            nd.canCollide = false;
            timer.onTimerEnd.AddListener(OnTimerEnd);
            wallPositions[0] = BossRoom.instance.roomBounds.bounds.max.x;
            wallPositions[1] = BossRoom.instance.roomBounds.bounds.min.x;
            wallPositions[2] = BossRoom.instance.roomBounds.bounds.max.z;
            wallPositions[3] = BossRoom.instance.roomBounds.bounds.min.z;
            gun = transform.position + Vector3.left;
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
        }

        
        void Update()
        {

        }

        private void setGunPoint()
        {
            if (player.transform.position.x > transform.position.x)
            {
                gun = transform.position + Vector3.right;
            }
            else
            {
                gun = transform.position + Vector3.left;
            }
        }

        public void OnTimerEnd(int data)
        {
            switch(data)
            {
                case DONE_POPPING_OUT_SINGLE:
                    movement.moveVelocity = Vector3.zero;
                    PopOutSingle();
                    break;
                case DONE_POPPING_OUT_SPREAD:
                    movement.moveVelocity = Vector3.zero;
                    PopOutSpread();
                    break;
                case KILL_DECOYS:
                    movement.moveVelocity = Vector3.zero;
                    nd.canCollide = false;
                    for(int i = 0; i<decoys.Length; i++)
                    {
                        Destroy(decoys[i]);
                    }
                    LayerMask Walls = LayerMask.GetMask("Walls");
                    GetComponent<BoxCollider>().excludeLayers = Walls;
                    break;
            }
        }

        private void OnDestroy() {
            foreach (GameObject d in decoys) {
                if(d != null) {
                    Destroy(d);
                }
            }
        }
    }
}
