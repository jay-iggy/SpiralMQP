using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{
    public float turnDelta;
    public float viewConeAngle;
    public bool facingTarget = false;

    public void TurnTowardsInstant(Vector3 target)
    {
        float opposite = target.z - transform.position.z;
        float adjacent = target.x - transform.position.x;
        if (adjacent == 0) adjacent = .01f; //divide by 0 protection

        float angle = Mathf.Atan(Mathf.Abs(opposite / adjacent));
        if (opposite < 0 && adjacent > 0)
        {
            angle = -angle;
        }
        else if (opposite < 0 && adjacent < 0)
        {
            angle += Mathf.PI;
        }
        else if (opposite > 0 && adjacent < 0)
        {
            angle = -angle;
            angle += Mathf.PI;
        }
        angle *= Mathf.Rad2Deg;
        angle *= -1;
        if (angle < 0) angle += 360;


        transform.eulerAngles = new Vector3(0, angle, 0);
    }

    public void TurnTowards(Vector3 target)
    {
        float opposite = target.z - transform.position.z;
        float adjacent = target.x - transform.position.x;
        if (adjacent == 0) adjacent = .01f; //divide by 0 protection

        float angle = Mathf.Atan(Mathf.Abs(opposite / adjacent));
        if (opposite < 0 && adjacent > 0)
        {
            angle = -angle;
        }
        else if (opposite < 0 && adjacent < 0)
        {
            angle += Mathf.PI;
        }
        else if (opposite > 0 && adjacent < 0)
        {
            angle = -angle;
            angle += Mathf.PI;
        }
        angle *= Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        float curAngle = -transform.localEulerAngles.y;
        if (curAngle < 0) curAngle += 360;
        if (curAngle > angle - turnDelta && curAngle < angle + turnDelta)
        {
            facingTarget = false;
            return;
        }

        float angleDistance = curAngle - angle;

        if (angleDistance > 180 || (angleDistance > -180 && angleDistance < 0)) //ccw
        {
            Quaternion deltaQ = new Quaternion();
            deltaQ.eulerAngles = new Vector3(0, -turnDelta, 0);
            transform.rotation *= deltaQ;
        }
        else //cw
        {
            Quaternion deltaQ = new Quaternion();
            deltaQ.eulerAngles = new Vector3(0, turnDelta, 0);
            transform.rotation *= deltaQ;
        }

        curAngle = -transform.localEulerAngles.y;
        if (curAngle < 0) curAngle += 360;
        if (curAngle > angle - viewConeAngle && curAngle < angle + viewConeAngle)
        {
            facingTarget = true;
        }
        else
        {
            facingTarget = false;
        }
    }

    public void MoveForward(float speed)
    {
        float angle = -transform.localEulerAngles.y * Mathf.Deg2Rad;
        float sine = Mathf.Sin(angle);
        float cosine = Mathf.Cos(angle);
        Vector3 forwardVelocity = new Vector3(cosine, 0, sine);
        forwardVelocity *= speed;
        transform.position += forwardVelocity;
    }
}
