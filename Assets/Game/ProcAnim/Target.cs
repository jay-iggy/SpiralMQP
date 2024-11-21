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
}
