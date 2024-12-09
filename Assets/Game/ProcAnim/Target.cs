using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public enum TargetType {
        home,
        target
    }
    public enum ObjType
    {
        Arm,
        Leg
    }

    public string relevantCharacter;
    public TargetType type;
    public ObjType relevantObj;
    public string rightOrLeft;

    private void OnEnable()
    {
        TargetManager.instance.addTarget(this);
    }
    private void OnDisable()
    {
        TargetManager.instance.removeTarget(this);
    }

    private void OnDrawGizmos()
    {
        if(type == TargetType.home)
            Gizmos.color = Color.green;
        else if(type == TargetType.target)
            Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(this.transform.position, 0.2f);
    }
}
