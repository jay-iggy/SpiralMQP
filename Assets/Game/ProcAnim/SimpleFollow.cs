using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    public Transform following;
    private static float followSpeed = 5f;
    private static float smoothness = 0.1f;
    [SerializeField] public float maxDistBtwn = 1;
    private static float rotSpeed = 5f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 diff = transform.position - following.position;
        diff.y = 0; // only interested in xz plane here

        if(diff.magnitude > maxDistBtwn)
        {
            // Smoothly interpolate the position of this segment
            Vector3 move = Vector3.SmoothDamp(transform.position, following.position, ref velocity, smoothness, followSpeed);
            move.y = this.transform.position.y;
            transform.position = move;

            // Match rotation of target
            Quaternion targetRotation = Quaternion.LookRotation(-diff.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }
    }
}
