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

    // Start is called before the first frame update
    void Start()
    {
        lastStepped = "right";
        lastArmTargetsCount = TargetManager.instance.armTargets.Count;
        StartCoroutine(OneLegAtATime());
        StartCoroutine(ArmsTrackItems());
    }
}
