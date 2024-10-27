using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    private static float rotSpeed = 25;
    [SerializeField] private Vector3 direction;

    public void FixedUpdate()
    {
        this.transform.Rotate(direction * rotSpeed * Time.deltaTime);
    }
}
