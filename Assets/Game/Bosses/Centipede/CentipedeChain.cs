using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class CentipedeChain : MonoBehaviour
    {
        public int numSegments;
        private int lastNumSegments;

        [SerializeField] public GameObject segmentPrefab;
        [SerializeField] public GameObject turretSegPrefab;
        [SerializeField] private GameObject lastSegment;
        private GameObject prevSegment;

        private GameObject lastSpawned;

        void Awake()
        {
            lastNumSegments = numSegments;
            lastSpawned = segmentPrefab;

            for (int i = 0; i < numSegments; i++)
                SpawnSegmentToEnd(segmentPrefab);
        }

        // Update is called once per frame
        void Update()
        {
            if (numSegments > lastNumSegments && numSegments < GetComponentInParent<CentipedeAttacks>().maxSegments)
            {
                for (int i = 0; i < numSegments - lastNumSegments; i++)
                {
                    if (lastSpawned.Equals(segmentPrefab))
                    {
                        SpawnSegmentToEnd(turretSegPrefab);
                        lastSpawned = turretSegPrefab;
                    }
                    else if (lastSpawned.Equals(turretSegPrefab))
                    {
                        SpawnSegmentToEnd(segmentPrefab);
                        lastSpawned = segmentPrefab;
                    }
                }

                lastNumSegments = numSegments;
            }
        }

        public void SpawnSegmentToEnd(GameObject seg)
        {
            float gap = 0;
            if (lastSegment.GetComponent<SimpleFollow>() != null)
            {
                gap = lastSegment.GetComponent<SimpleFollow>().maxDistBtwn;
            }

            Vector3 newSegmentPos = lastSegment.transform.position - (lastSegment.transform.forward * gap);
            newSegmentPos.y = .3f;
            GameObject newSegment = Instantiate(seg, newSegmentPos, lastSegment.transform.rotation, this.transform);

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
}
