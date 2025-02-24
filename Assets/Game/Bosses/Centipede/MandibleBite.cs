using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts;

public class MandibleBite : MonoBehaviour
{
    public Transform m1;
    public Transform m2;

    private void OnTriggerEnter(Collider other)
    {
        CentipedeChain cc = GetComponentInParent<CentipedeChain>();

        if (other.gameObject.name.Contains("CentipedeSnack"))
        {
            Destroy(other.gameObject);
            if (cc.numSegments + 1 < GetComponentInParent<CentipedeAttacks>().maxSegments)
                cc.numSegments++;

            GetComponentInParent<CentipedeAttacks>().target = "player";
            // GetComponentInParent<HealthComponent>().SetHealth(GetComponentInParent<HealthComponent>().health + 1);
        }
    }
}
