using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTag : MonoBehaviour
{
    public string tag;
    public Transform look;
    public int flip = 1; // 1 or -1

    void Start()
    {
        look = GameObject.FindWithTag(tag).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (look != null)
        {
            Vector3 targetDirection = look.transform.position - this.transform.position;
            this.transform.rotation = Quaternion.LookRotation(targetDirection * flip);
        }
    }
}
