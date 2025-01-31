using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    [SerializeField] float seconds;
    [SerializeField] SpriteRenderer spr;
    public bool active = true;
    private float timer;
    private float alpha = 1;

    public void setSeconds(float s)
    {
        seconds = s;
        active = true;
    }

    void Update()
    {
        timer = seconds;
        if (!active) return;

        spr.color = new Color(1, 1, 1, alpha);
        alpha -= Time.deltaTime / seconds;

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
