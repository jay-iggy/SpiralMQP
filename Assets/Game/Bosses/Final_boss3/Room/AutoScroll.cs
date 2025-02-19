using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(-5, 0, 0); // Set manually
    public float speed = 2f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            enabled = false;
        }
    }
}
