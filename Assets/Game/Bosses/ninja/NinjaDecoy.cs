using Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaDecoy : MonoBehaviour
{
    [SerializeField] GameObject smoke;
    private MovementComponent movement;
    private float angle = 0;
    private float speed = 5;
    public bool canCollide = true;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void setRandomPosition(float[] bounds, float internalOffset, float yPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag(TagManager.Player);
        movement = GetComponent<MovementComponent>();
        float xPos = player.transform.position.x;
        float zPos = player.transform.position.z;
        while (xPos > player.transform.position.x - internalOffset && xPos < player.transform.position.x + internalOffset && zPos > player.transform.position.z - internalOffset && zPos < player.transform.position.z + internalOffset)
        {
            xPos = Random.Range(bounds[1] + internalOffset, bounds[0] - internalOffset);
            zPos = Random.Range(bounds[3] + internalOffset, bounds[2] - internalOffset);
        }       
        transform.position = new Vector3(xPos, yPos, zPos);
        GameObject spawnSmoke = Instantiate(smoke, transform.position, Quaternion.identity);
        spawnSmoke.GetComponent<FadeAway>().setSeconds(.5f);
        angle = Random.Range(0, 2*Mathf.PI);
        Debug.Log("angle: " + angle);
        setVelocity();
    }

    public void setVelocity()
    {
        movement.moveVelocity = new Vector3(Mathf.Cos(angle)*speed, 0, Mathf.Sin(angle)*speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canCollide) return;

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<HealthComponent>().GetHit(1);
        }
        else if(other.gameObject.tag == "Wall")
        {
            if(other.gameObject.name == "H")
            {
                if (angle > Mathf.PI)
                {
                    angle = Random.Range(Mathf.PI / 6, Mathf.PI * 5/6);
                }
                else
                {
                    angle = Random.Range(Mathf.PI * 7/6, Mathf.PI * 11/6);
                }
                setVelocity();
            }
            else if(other.gameObject.name == "V")
            {
                if(angle > Mathf.PI/2 && angle <= Mathf.PI * 3 / 2)
                {
                    angle = Random.Range(Mathf.PI * 10 / 6, Mathf.PI * 14 / 6);
                }
                else
                {                   
                    angle = Random.Range(Mathf.PI * 4 / 6, Mathf.PI * 8 / 6);
                }
                setVelocity();
            }
        }
    }
}
