using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts;

public class LegStepper : MonoBehaviour
{      
    public Transform defaulttt;
    public Transform target;
    public float distToStep;
    public static float timeMoving = 0.05f;
    public bool moving = false;
    private float timer;

    public bool masterLeg; // for pair
    [SerializeField] private LegStepper otherLeg;
    private string lastStepped;

    public void Move()
    {
        if(Vector3.Distance(defaulttt.position, target.position) > distToStep)
        {
            target.position = defaulttt.position;
            timer = timeMoving;
            moving = true;
        }
    }

    public void Update()
    {
        if (moving)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                moving = false;
            }
        }
    }

    public void onTimerEnd(int data)
    {
        moving = false;
    }

    IEnumerator OneLegAtATime()
    {
        // always running
        while (true)
        {
            if (lastStepped.Equals("this") && !this.moving && !otherLeg.moving)
            {
                otherLeg.Move();
                lastStepped = "other";
                yield return null;
            }
            else if (lastStepped.Equals("other") && !otherLeg.moving && !this.moving)
            {
                this.Move();
                lastStepped = "this";
                yield return null;
            }

            yield return null;
        }
    }
    private void Start()
    {
        lastStepped = "this";
        if (masterLeg)
            StartCoroutine(OneLegAtATime());
    }
}
