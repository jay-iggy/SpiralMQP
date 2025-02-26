using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class NewItemUnlocked : MonoBehaviour
{
    public UnityEvent onDoneDisplaying;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] GameObject iconPos;
    private float timer = -1;
    private float yVelocity = 200;

    public void SetItem(ItemPickup item)
    {
        itemName.text = item.itemName;
        GameObject itemModel = Instantiate(item, transform).gameObject;
        itemModel.layer = 5; //UI
        foreach (MeshRenderer child in itemModel.GetComponentsInChildren<MeshRenderer>(true)) {
            child.gameObject.layer = 5;
        }
        // go through all children until there are no more children and set the layer to UI
        
        itemModel.transform.position = iconPos.transform.position;
        itemModel.transform.localScale = new Vector3(100, 100, 100);
        itemModel.transform.eulerAngles = new Vector3(-90, 0, 0);
    }
    
    void Start()
    {
        
    }

    private void Update()
    {
        transform.localPosition += new Vector3(0, yVelocity*Time.deltaTime, 0);
        if (transform.localPosition.y >= -440 && timer == -1)
        {
            yVelocity = 0;
            timer = 0;
        }
        if (transform.localPosition.y < -641)
        {
            onDoneDisplaying.Invoke();
            Destroy(gameObject);
        }

        if (timer >= 0)
        {
            timer += Time.deltaTime;
            if(timer >= 1.5f)
            {
                yVelocity = -200;
                timer = -1;
            }
        }
    }
}
