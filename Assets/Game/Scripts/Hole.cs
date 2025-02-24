using Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hole : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    public GameObject Camera;

    public UnityEvent ChangeRoom = new();
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(TagManager.Player);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("fall");
            Fall();
        }
    }

    private void Fall()
    {
        player.transform.position = player.transform.position += new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z+100);

        ChangeRoom.Invoke();
    }


}
