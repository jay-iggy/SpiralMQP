using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PranimDriver : MonoBehaviour
{
    #region legs
    [SerializeField] private LegStepper rightLeg;
    [SerializeField] private LegStepper leftLeg;
    private string lastStepped;

    IEnumerator OneLegAtATime()
    {
        // always running
        while (true)
        {
            if (lastStepped.Equals("right") && !rightLeg.moving && !leftLeg.moving)
            {
                leftLeg.Move();
                lastStepped = "left";
                yield return null;
            }
            else if (lastStepped.Equals("left") && !leftLeg.moving && !rightLeg.moving)
            {
                rightLeg.Move();
                lastStepped = "right";
                yield return null;
            }

            yield return null;
        }
    }
    #endregion

    #region arms
    [SerializeField] private InverseKinematics rightArm;
    [SerializeField] private InverseKinematics leftArm;

    private int lastArmTargetsCount;

    IEnumerator ArmsTrackItems()
    {
        while (true)
        {
            // Check for changes in armTargets count
            if (TargetManager.instance.armTargets.Count != lastArmTargetsCount)
            {
                if (TargetManager.instance.armTargets.Count > 0)
                {
                    // Assign targets to arms
                    foreach (var t in TargetManager.instance.armTargets)
                    {
                        if (t.rightOrLeft == "right")
                            rightArm.target = t.transform;
                        else if (t.rightOrLeft == "left")
                            leftArm.target = t.transform;
                    }
                }
                else
                {
                    // Assign default/home targets
                    foreach (var t in TargetManager.instance.armHomes)
                    {
                        if (t.rightOrLeft == "right")
                            rightArm.target = t.transform;
                        else if (t.rightOrLeft == "left")
                            leftArm.target = t.transform;
                    }
                }

                // Update the last known count
                lastArmTargetsCount = TargetManager.instance.armTargets.Count;
            }

            // Delay the coroutine to prevent excessive execution
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region materials
    [SerializeField] private Material skin;
    [SerializeField] private Material shirt;
    [SerializeField] private Material pants;
    [SerializeField] private Material hair;

    public void UpdateMaterialsToDefaults()
    {
        // Legs
        // right thigh
        rightLeg.transform.GetChild(1).GetComponent<MeshRenderer>().material = pants;
        // right calf
        rightLeg.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = pants;
        // right foot
        rightLeg.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = pants;
        // left thigh
        leftLeg.transform.GetChild(1).GetComponent<MeshRenderer>().material = pants;
        // left calf
        leftLeg.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = pants;
        // left foot
        leftLeg.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = pants;

        // Arms
        // right sleeve
        rightArm.transform.GetChild(1).GetComponent<MeshRenderer>().material = shirt;
        // right forearm
        rightArm.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = skin;
        // right hand
        rightArm.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = skin;
        // left sleeve
        leftArm.transform.GetChild(1).GetComponent<MeshRenderer>().material = shirt;
        // left forearm
        leftArm.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = skin;
        // left hand
        leftArm.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = skin;

        // Body
        // hip box
        this.transform.GetChild(1).GetComponent<MeshRenderer>().material = pants;
        // torso
        this.transform.GetChild(2).GetComponent<MeshRenderer>().material = shirt;
        // chest
        this.transform.GetChild(3).GetComponent<MeshRenderer>().material = shirt;
        // head
        this.transform.GetChild(6).GetComponent<MeshRenderer>().material = skin;
        // hair
        this.transform.GetChild(7).GetComponent<MeshRenderer>().material = hair;

    }

    public void SetAllMaterialsToOneMat(Material m)
    {
        // Legs
        // right thigh
        rightLeg.transform.GetChild(1).GetComponent<MeshRenderer>().material = m;
        // right calf
        rightLeg.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = m;
        // right foot
        rightLeg.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = m;
        // left thigh
        leftLeg.transform.GetChild(1).GetComponent<MeshRenderer>().material = m;
        // left calf
        leftLeg.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = m;
        // left foot
        leftLeg.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = m;

        // Arms
        // right sleeve
        rightArm.transform.GetChild(1).GetComponent<MeshRenderer>().material = m;
        // right forearm
        rightArm.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = m;
        // right hand
        rightArm.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = m;
        // left sleeve
        leftArm.transform.GetChild(1).GetComponent<MeshRenderer>().material = m;
        // left forearm
        leftArm.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = m;
        // left hand
        leftArm.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = m;

        // Body
        // hip box
        this.transform.GetChild(1).GetComponent<MeshRenderer>().material = m;
        // torso
        this.transform.GetChild(2).GetComponent<MeshRenderer>().material = m;
        // chest
        this.transform.GetChild(3).GetComponent<MeshRenderer>().material = m;
        // head
        this.transform.GetChild(6).GetComponent<MeshRenderer>().material = m;
        // hair
        this.transform.GetChild(7).GetComponent<MeshRenderer>().material = m;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        lastStepped = "right";
        lastArmTargetsCount = TargetManager.instance.armTargets.Count;
        StartCoroutine(OneLegAtATime());
        StartCoroutine(ArmsTrackItems());
    }
}
