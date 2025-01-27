using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotToTag : MonoBehaviour
{
    public string tag;
    public Transform look;
    public int flip = 1; // 1 or -1

    // Start is called before the first frame update
    void Start()
    {
        look = GameObject.FindWithTag(tag).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (look != null)
        {
            // Calculate the direction to the look target
            Vector3 targetDirection = look.transform.position - this.transform.position;

            // Zero out the y-component of the targetDirection to ignore vertical rotation
            targetDirection.y = 0;

            // Reverse the direction to make the blue arrow face the opposite way
            targetDirection *= flip;

            // Rotate only around the y-axis to face the target
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Apply the new rotation while keeping the current rotation on the x and z axes
            this.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }
    }
}
