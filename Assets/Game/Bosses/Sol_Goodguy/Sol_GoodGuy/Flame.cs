using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    private float countdown = 5f; // 5 seconds
    [SerializeField] GameObject bullet;

    private float timer = 0f;

    void FixedUpdate()
    {
        countdown -= Time.deltaTime; // Subtract elapsed time
        timer += Time.deltaTime; // Add the time since the last frame
        
        if (timer >= 0.25f) // Check if 1 second has passed
        {
            setTrail();
            countdown -= 0.25f; // Subtract 1 from countdown
            timer = 0f; // Reset the timer
        }

        // Optional: Stop at 0 to prevent negative values
        if (countdown <= 0)
        {
            countdown = 0;
            Destroy(gameObject);
        }
    }
    private void Start()
    {

    }
    public void setTrail()
    {
        GameObject[] bullets = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            bullets[i] = Instantiate(bullet, transform.position, transform.rotation);
        }
            
    }

}
