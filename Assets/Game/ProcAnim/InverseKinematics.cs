using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public Transform target; // the position to reach toward
    [SerializeField] Transform pole; // elbow / knee, the middle joint

    [SerializeField] Transform firstBone;
    [SerializeField] Vector3 firstBoneEulerAngleOffset;
    [SerializeField] Transform secondBone;
    [SerializeField] Vector3 secondBoneEulerAngleOffset;
    [SerializeField] Transform thirdBone;
    [SerializeField] Vector3 thirdBoneEulerAngleOffset;
    [SerializeField] bool alignThirdBoneWithTargetRotation = true;

    void OnEnable()
    {
        // debug if bones not initialized
        if (
            firstBone == null ||
            secondBone == null ||
            thirdBone == null ||
            pole == null ||
            target == null
        )
        {
            Debug.LogError("1+ IK bones are null", this);
            enabled = false;
            return;
        }
    }

    void LateUpdate()
    {
        Vector3 towardPole = pole.position - firstBone.position;
        Vector3 towardTarget = target.position - firstBone.position;

        float rootBoneLength = Vector3.Distance(firstBone.position, secondBone.position);
        float secondBoneLength = Vector3.Distance(secondBone.position, thirdBone.position);
        float totalChainLength = rootBoneLength + secondBoneLength;

        // Align root with target
        firstBone.rotation = Quaternion.LookRotation(towardTarget, towardPole);
        firstBone.localRotation *= Quaternion.Euler(firstBoneEulerAngleOffset);

        Vector3 towardSecondBone = secondBone.position - firstBone.position;

        var targetDistance = Vector3.Distance(firstBone.position, target.position);

        // Limit hypotenuse to under the total bone distance to prevent invalid triangles
        targetDistance = Mathf.Min(targetDistance, totalChainLength * 0.9999f);

        // Solve for the angle for the root bone (Law of Cosines)
        var adjacent =
            (
                (rootBoneLength * rootBoneLength) +
                (targetDistance * targetDistance) -
                (secondBoneLength * secondBoneLength)
            ) / (2 * targetDistance * rootBoneLength);
        var angle = Mathf.Acos(adjacent) * Mathf.Rad2Deg;

        // rotate around the vector orthogonal to both pole and second bone
        Vector3 cross = Vector3.Cross(towardPole, towardSecondBone);

        // rotate only if angle is valid
        if (!float.IsNaN(angle))
        {
            firstBone.RotateAround(firstBone.position, cross, -angle);
        }

        // Root bone is already rotated correctly,
        // now look at target from elbow for final rot
        var secondBoneTargetRotation = Quaternion.LookRotation(target.position - secondBone.position, cross);
        secondBoneTargetRotation *= Quaternion.Euler(secondBoneEulerAngleOffset);
        secondBone.rotation = secondBoneTargetRotation;

        // align 3rd bone if desired
        if (alignThirdBoneWithTargetRotation)
        {
            thirdBone.rotation = target.rotation;
            thirdBone.localRotation *= Quaternion.Euler(thirdBoneEulerAngleOffset);
        }
    }
}
