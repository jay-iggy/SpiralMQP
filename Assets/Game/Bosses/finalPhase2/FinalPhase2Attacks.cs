using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts {
    public class FinalPhase2Attacks : MonoBehaviour, ICanAttack
    {
        private const int MACHINE_LASER = 0;
        private const int RAT_CIRCLE = 1;
        private const int UPDATE_RAT_CIRCLE = -1;
        private const int FROG_JUMP = 2;
        private const int TRITON_THROW = 3;
        private const int RESET_TRIDENT = -3;
        private const int NINJA_TP = 4;
        private const int DONE_POPPING_OUT_SINGLE = -4;
        private const int DONE_POPPING_OUT_SPREAD = -5;
        private const int BASILISK_TONGUE = 5;
        private const int SOL_FIRECONE = 6;
        private const int MACHINE_SMOKE = 7;

        private int numAttacks = 8;
        private int curAttack;

        [SerializeField] Timer timer;

        private GameObject player;
        [SerializeField] GameObject bullet;

        private int numMovesSinceMovement = 0;
        private bool tping;
        private int numTPs = 0;

        // MACHINE_LASER & SMOKE
        [SerializeField] GameObject laser;
        float laserTurnDelta = 0;
        [SerializeField] GameObject smokePrefab;

        // RAT_CIRCLE
        GameObject[] bullets = new GameObject[12]; // why not just use a list?

        // FROG_JUMP
        private bool _isGrounded = false;
        private MovementComponent _movementComponent;

        private int projectileCount = 8;
        private float projectileCircleRadius = 2;
        private float projectileSpeed = 10;
        public float jumpForce = 20f;
        private float speed = 10;

        // TRITON_THROW
        [SerializeField] GameObject[] tridents; //prefab, default, up, left, right, thrown

        // NINJA_TP
        private float[] wallPositions = new float[4];
        [SerializeField] float startDistanceBehindWall;
        [SerializeField] float popOutSpeed;
        private float minDistanceFromEdge = 2;
        int prevWall = -1;

        // BASILISK_TONGUE
        [SerializeField] Transform tonguePoint;
        private LineRenderer tongue;
        private Vector3 tongueEnd;
        private Vector3 tongueTarget;
        private float percentToTarget = 0; // [0,1]
        private float percentIncrement = 0.05f;
        private bool tongueStuck = false;
        private bool retractingTongue = false;
        private bool completedTongueAttack = false;

        // SOL_FIRECONE
        [SerializeField] GameObject ExplosiveFlame;
        [SerializeField] GameObject Flame;


        public int GetAttackCount() { return numAttacks; }

        public float Attack(int index)
        {
            Debug.Log(index);

            // reset for MACHINE_LASER
            laser.SetActive(false);
            laserTurnDelta = 0;

            // always tp multiple times in a row
            if (tping)
            {
                float rand = Random.Range(0, 1);
                if (rand > 0.5)
                    PopOut(DONE_POPPING_OUT_SINGLE);
                else
                    PopOut(DONE_POPPING_OUT_SPREAD);

                numTPs++;
                if (numTPs >= 3)
                    tping = false;

                return 0;
            }
            if (numMovesSinceMovement > 3)
            {
                float rand = Random.Range(0, 1);

                if (rand < 0.5)
                {
                    Vector3 targetPosition = BossRoom.GetRandomPositionInRoom(5);
                    StartCoroutine(JumpTo(targetPosition));
                }
                else
                {
                    tping = true;
                    numTPs++;
                    if (numTPs >= 3)
                        tping = false;

                    float rand2 = Random.Range(0, 1);
                    if (rand2 > 0.5)
                        PopOut(DONE_POPPING_OUT_SINGLE);
                    else
                        PopOut(DONE_POPPING_OUT_SPREAD);
                }

                numMovesSinceMovement = 0;
                return 0;
            }

            switch (index)
            {
                case RAT_CIRCLE:
                    ShootCirclePattern();
                    numMovesSinceMovement++;
                    break;
                case MACHINE_LASER:
                    GoToCenter();
                    numMovesSinceMovement++;
                    break;
                case FROG_JUMP:
                    Vector3 targetPosition = BossRoom.GetRandomPositionInRoom(5);
                    StartCoroutine(JumpTo(targetPosition));
                    numMovesSinceMovement = 0;
                    break;
                case TRITON_THROW:
                    ThrowTrident();
                    numMovesSinceMovement++;
                    break;
                case NINJA_TP:
                    tping = true;
                    numTPs++;
                    if (numTPs >= 3)
                        tping = false;

                    float rand = Random.Range(0, 1);
                    if(rand > 0.5)
                        PopOut(DONE_POPPING_OUT_SINGLE);
                    else
                        PopOut(DONE_POPPING_OUT_SPREAD);

                    numMovesSinceMovement = 0;
                    break;
                case BASILISK_TONGUE:
                    Tongue();
                    numMovesSinceMovement++;
                    break;
                case SOL_FIRECONE:
                    GunFlame();
                    numMovesSinceMovement++;
                    break;
                case MACHINE_SMOKE:
                    MakeSmoke();
                    numMovesSinceMovement++;
                    break;
            }

            return 0;
        }

        void Start()
        {
            timer.onTimerEnd.AddListener(OnTimerEnd);
            player = GameObject.FindGameObjectWithTag(TagManager.Player);

            _movementComponent = GetComponent<MovementComponent>();

            wallPositions[0] = BossRoom.instance.roomBounds.bounds.max.x;
            wallPositions[1] = BossRoom.instance.roomBounds.bounds.min.x;
            wallPositions[2] = BossRoom.instance.roomBounds.bounds.max.z;
            wallPositions[3] = BossRoom.instance.roomBounds.bounds.min.z;

            tongue = this.GetComponent<LineRenderer>();
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case MACHINE_LASER:
                    Debug.Log("shoot");
                    ShootLaser();
                    break;
                case RAT_CIRCLE:
                    ShootCirclePattern();
                    break;
                case UPDATE_RAT_CIRCLE:
                    BulletPatterns.MoveTowards(bullets, transform.position, -8);
                    foreach (GameObject b in bullets)
                    {
                        if (b != null)
                        {
                            b.GetComponent<Projectile>().destroyedByWall = true;
                        }
                    }
                    bullets = new GameObject[12];
                    break;
                case RESET_TRIDENT:
                    if (tridents[5] != null) Destroy(tridents[5]);

                    for (int i = 2; i < 5; i++)
                    {
                        tridents[i].SetActive(false);
                    }
                    // tridents[1].SetActive(true);
                    break;
                case DONE_POPPING_OUT_SINGLE:
                    _movementComponent.moveVelocity = Vector3.zero;
                    PopOutSingle();
                    break;
                case DONE_POPPING_OUT_SPREAD:
                    _movementComponent.moveVelocity = Vector3.zero;
                    PopOutSpread();
                    break;

            }
        }

        void FixedUpdate()
        {
            // MACHINE_LASER
            Quaternion laserQuat = new Quaternion();
            laserQuat.eulerAngles = new Vector3(0, laserTurnDelta, 0);
            laser.transform.rotation *= laserQuat;

            // BASILISK_TONGUE
            if (!completedTongueAttack)
            {
                if (!tongueStuck && Vector3.Distance(player.transform.position, tongueEnd) < 1) // check if hit player 
                    tongueStuck = true;
                if (percentToTarget >= 1) // check maxedLength
                    retractingTongue = true;

                if (retractingTongue || tongueStuck) // retract
                {
                    if (percentToTarget < 0) // end when fully retracted
                    {
                        UnrenderTongue();
                        percentToTarget = 0;
                        retractingTongue = false;
                        tongueStuck = false;
                        completedTongueAttack = true;
                        this.GetComponent<Boss>().DoneWithAttack();
                        return;
                    }

                    percentToTarget -= percentIncrement;
                    tongueEnd = Vector3.Lerp(tonguePoint.position, tongueTarget, percentToTarget);
                    RenderTongue(tonguePoint.position, tongueEnd);

                    if (tongueStuck)
                        player.transform.position = tongueEnd;
                }
                else // extend
                {
                    tongueEnd = Vector3.Lerp(tonguePoint.position, tongueTarget, percentToTarget);
                    percentToTarget += percentIncrement;
                    RenderTongue(tonguePoint.position, tongueEnd);
                }
            }
        }

        private void GoToCenter()
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, 0), Vector3.Distance(new Vector3(0, 0, 0), transform.position) / 50);
            Debug.Log(curAttack);
            timer.Set(0.1f, curAttack);

        }

        // RAT_CIRCLE
        private void ShootCirclePattern()
        {
            for (int i = 0; i < 12; i++)
            {
                bullets[i] = Instantiate(bullet);
            }
            BulletPatterns.CreateCircle(bullets, transform.position, 1);
            timer.Set(.25f, UPDATE_RAT_CIRCLE);
        }

        // MACHINE_LASER
        private float ShootLaser()
        {
            float adjacent = player.transform.position.x;
            float opposite = player.transform.position.z;
            if (adjacent == 0) adjacent = .01f; //divide by 0 protection

            float angle = Mathf.Atan(Mathf.Abs(opposite / adjacent));
            if (opposite > 0 && adjacent > 0)
            {
                angle = -angle;
            }
            else if (opposite > 0 && adjacent < 0)
            {
                angle += Mathf.PI;
            }
            else if (opposite < 0 && adjacent < 0)
            {
                angle = -angle;
                angle += Mathf.PI;
            }
            angle *= Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            laser.transform.localEulerAngles = new Vector3(0, angle - 90, 0); //starts opposite player
            if (Random.Range(0, 2) == 0)
            {
                laserTurnDelta = 2;
            }
            else
            {
                laserTurnDelta = -2;
            }

            laser.SetActive(true);

            return 2f;
        }

        // FROG_JUMP
        IEnumerator JumpTo(Vector3 target)
        {
            _isGrounded = false;
            _movementComponent.AddVerticalVelocity(jumpForce);
            _movementComponent.moveVelocity = (target - transform.position).normalized * speed;
            while (!_isGrounded)
            {
                yield return null;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(TagManager.Ground))
            {
                _movementComponent.moveVelocity = Vector3.zero;
                GameObject[] projectiles = new GameObject[projectileCount];
                for (int i = 0; i < projectileCount; i++)
                {
                    projectiles[i] = Instantiate(bullet, transform.position, Quaternion.identity);
                }
                BulletPatterns.CreateCircle(projectiles, transform.position, projectileCircleRadius);
                foreach (GameObject projectile in projectiles)
                {
                    projectile.GetComponent<Rigidbody>().velocity = (projectile.transform.position - transform.position).normalized * projectileSpeed;
                }
                _isGrounded = true;
            }
            else if (other.gameObject.CompareTag(TagManager.Player))
            {
                MovementComponent playerMovementComponent = other.gameObject.GetComponent<MovementComponent>();
                if (playerMovementComponent != null)
                {
                    //move the player to the side so the frog can land
                    Vector3 direction = (other.transform.position - transform.position).normalized;
                    direction.y = 0;
                    playerMovementComponent.AddExternalVelocity(direction * 4);
                }
            }
        }

        // TRITON_THROW
        private void ThrowTrident()
        {
            tridents[5] = Instantiate(tridents[0]);
            tridents[5].transform.position = this.transform.position;
            tridents[5].GetComponent<TurnHandler>().TurnTowardsInstant(player.transform.position);
            tridents[5].GetComponent<Projectile>().TargetPlayer(12);
            timer.Set(1, RESET_TRIDENT);
        }

        // NINJA_TP
        private void PopOutSingle()
        {
            GameObject singleBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            singleBullet.GetComponent<Projectile>().destroyedByWall = false;
            BulletPatterns.MoveTowards(singleBullet, player.transform.position, 8);
        }

        private void PopOutSpread()
        {
            GameObject[] bulletSpread = new GameObject[12];
            for (int i = 0; i < bulletSpread.Length; i++)
            {
                bulletSpread[i] = Instantiate(bullet);
                bulletSpread[i].GetComponent<Projectile>().destroyedByWall = false;
            }
            BulletPatterns.CreateCircle(bulletSpread, transform.position, .05f);
            BulletPatterns.MoveTowards(bulletSpread, transform.position, -7);
        }

        private float PopOut(int whatToDoAfter)
        {
            int whichWall = prevWall;
            while (whichWall == prevWall)
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
            _movementComponent.moveVelocity = popOutVelocity;
            timer.Set(.2f, whatToDoAfter);

            if (whatToDoAfter == DONE_POPPING_OUT_SINGLE) return .75f;
            else if (whatToDoAfter == DONE_POPPING_OUT_SPREAD) return 1.75f;
            else return 0;
        }

        // BASILISK_TONGUE
        private float Tongue()
        {
            tongueTarget = player.transform.position;
            completedTongueAttack = false;
            return -1;
        }
        public void RenderTongue(Vector3 start, Vector3 end)
        {
            tongue.positionCount = 2;
            Vector3[] points = { start, end };
            tongue.SetPositions(points);
        }
        public void UnrenderTongue()
        {
            tongue.positionCount = 0;
        }

        // SOL_FIRECONE
        private float GunFlame()
        {
            int bulletCount = 5;
            float coneAngle = 30f;
            float bulletSpeed = 5f;


            GameObject[] bullets = new GameObject[bulletCount];
            Vector3 origin = transform.position;
            Vector3 baseDirection = (player.transform.position - origin).normalized;


            for (int i = 0; i < bulletCount; i++)
            {
                // Instantiate bullet at the current position
                if (Random.Range(0,1) > 0.5f)
                {

                    bullets[i] = Instantiate(ExplosiveFlame, origin, Quaternion.identity);
                }
                else
                {
                    bullets[i] = Instantiate(Flame, origin, Quaternion.identity);
                }


                // Calculate spread angle for each bullet
                float angleOffset = ((i / (float)(bulletCount - 1)) - 0.5f) * coneAngle;

                // Rotate the base direction by the computed angle
                Vector3 spreadDirection = Quaternion.Euler(0, angleOffset, 0) * baseDirection;

                // Apply velocity or movement logic
                Rigidbody rb = bullets[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = spreadDirection * bulletSpeed;
                }
            }

            return 1;
        }

        // MACHINE_SMOKE
        private float MakeSmoke()
        {
            GameObject smoke = Instantiate(smokePrefab);
            MachineSmoke ms = smoke.GetComponent<MachineSmoke>();

            switch (Random.Range(0, 5))
            {
                case 0:
                    smoke.transform.position = this.transform.position;
                    ms.GoTo(this.transform.position, new Vector3(12.4f, 1, 6.2f));
                    break;
                case 1:
                    smoke.transform.position = new Vector3(-1.88f, .4f, 0);
                    ms.GoTo(new Vector3(-6, 0, 0), new Vector3(8, 1, 11.1f));
                    break;
                case 2:
                    smoke.transform.position = new Vector3(1.88f, .4f, 0);
                    ms.GoTo(new Vector3(6, 0, 0), new Vector3(8, 1, 11.1f));
                    break;
                case 3:
                    smoke.transform.position = new Vector3(0, .4f, -1.3f);
                    ms.GoTo(new Vector3(0, 0, -3.4f), new Vector3(20.7f, 1, 3.7f));
                    break;
                case 4:
                    smoke.transform.position = new Vector3(0, .4f, 1.3f);
                    ms.GoTo(new Vector3(0, 0, 3.4f), new Vector3(20.7f, 1, 3.7f));
                    break;
            }

            return 1;
        }
    }
}
