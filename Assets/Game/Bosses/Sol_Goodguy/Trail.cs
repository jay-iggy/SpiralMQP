using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] float countdown = 5f; // 5 seconds
    private float timer = 0f;

    void Update()
    {
        countdown -= Time.deltaTime; // Subtract elapsed time
        timer += Time.deltaTime; // Add the time since the last frame
        if (timer >= 1f) // Check if 1 second has passed
        {
            countdown -= 1f; // Subtract 1 from countdown
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
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;      // Stop movement
            rb.angularVelocity = Vector3.zero;  // Stop rotation
        }
    }

}
