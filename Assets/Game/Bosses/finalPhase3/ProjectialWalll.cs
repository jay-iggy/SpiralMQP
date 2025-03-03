using Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectialWalll : Projectile
{


    private float timer = 0f;
    public bool timerFinished = false;
    private bool StartTimer = false;
    public SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>(); // Get the SphereCollider component
    }

    void Update()
    {
        if (StartTimer)
        {
            timer += Time.deltaTime;

            if (timer >= 2f) // 2 second has passed
            {
                TimerComplete();
                timer = 0f; // Reset timer if you want it to repeat (optional)
            }
        }

    }

    void TimerComplete()
    {
        sphereCollider.enabled = true;
    }
    public override void DestroySelf()
    {
        sphereCollider.enabled = false;
        StartTimer = true;
        timer = 0f;
    }
}
