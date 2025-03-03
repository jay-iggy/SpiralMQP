using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScroll: MonoBehaviour
{

    private float speed = 1f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.right, speed * Time.deltaTime);

    }
}
