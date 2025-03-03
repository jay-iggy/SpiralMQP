using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustBox : MonoBehaviour
{
    public List<GameObject> Boxes = new List<GameObject>(); // Create an empty list
    public GameObject Backwall;
    public float pading = 21f;

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Boxes[0].transform.position.x < Backwall.transform.position.x)
        {
            MoveFirstToLast();
        }
    }

    void MoveFirstToLast()
    {
        Boxes[0].transform.position = new Vector3(Boxes[2].transform.position.x + pading, Boxes[0].transform.position.y, Boxes[0].transform.position.z);

        if (Boxes.Count > 1) // Ensure there's more than one object
        {
            GameObject first = Boxes[0]; // Store the first element
            Boxes.RemoveAt(0); // Remove it from the front
            Boxes.Add(first); // Add it to the back
        }
    }
}
