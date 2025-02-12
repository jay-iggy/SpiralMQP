using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeChain : MonoBehaviour
{
    public int numSegments;
    private int lastNumSegments;

    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private GameObject lastSegment;
    private GameObject prevSegment;

    void Awake()
    {
        lastNumSegments = numSegments;
        

        for (int i = 0; i < numSegments; i++)
            SpawnSegmentToEnd();
    }

    // Update is called once per frame
    void Update()
    {
        if(numSegments > lastNumSegments)
        {
            for(int i = 0; i < numSegments - lastNumSegments; i++)
                SpawnSegmentToEnd();

            lastNumSegments = numSegments;
        }
    }

    public void SpawnSegmentToEnd() {
        float gap = 0;
        if(lastSegment.GetComponent<SimpleFollow>() != null) {
            gap = lastSegment.GetComponent<SimpleFollow>().maxDistBtwn;
        }
        
        Vector3 newSegmentPos = lastSegment.transform.position - (lastSegment.transform.forward * gap);
        newSegmentPos.y = .3f;
        GameObject newSegment = Instantiate(segmentPrefab, newSegmentPos, lastSegment.transform.rotation, this.transform);

        // Set the new segment to follow the previous last segment
        newSegment.GetComponentInChildren<SimpleFollow>().following = lastSegment.transform.GetChild(0).transform;

        // Update lastSegment reference to this newly created segment
        prevSegment = lastSegment;
        lastSegment = newSegment;
    }

    public void RemoveLastSegment()
    {
        Destroy(lastSegment);

        numSegments--;
        lastNumSegments--;

        lastSegment = prevSegment;
    }
}
