using Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveFlame : MonoBehaviour
{
    private float countdown = 3f; // 5 seconds
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject Flame;
    private GameObject player;
    GameObject[] bullets = new GameObject[12];
    [SerializeField] private GameObject explosionEffectPrefab;

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
        if (countdown <= 0)
        {
            Explode();
        }
        // Optional: Stop at 0 to prevent negative values

    }

    void Explode()
    {
            for (int i = 0; i < 12; i++)
            {
                bullets[i] = Instantiate(Flame);
            }
            BulletPatterns.CreateCircle(bullets, transform.position, 1);
            BulletPatterns.MoveTowards(bullets, transform.position, -3);
            
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Check if the other object has the "Wall" tag
        if (collision.gameObject.CompareTag("Wall"))
        {
            Explode();

        }
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton

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
