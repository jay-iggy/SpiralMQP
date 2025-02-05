using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandibleBite : MonoBehaviour
{
    public Transform m1;
    public Transform m2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("CentipedeSnack"))
        {
            Destroy(other.gameObject);
            GetComponentInParent<CentipedeChain>().SpawnSegmentToEnd();
            GetComponentInParent<Game.Scripts.CentipedeAttacks>().target = "player";
        }
    }
}
