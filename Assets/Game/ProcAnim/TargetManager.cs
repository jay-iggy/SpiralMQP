using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager instance;
    public List<Target> legHomes;
    public List<Target> legTargets;

    public List<Target> armHomes;
    public List<Target> armTargets;

    void Awake()
    {
        instance = this;

        legHomes = new List<Target>();
        legTargets = new List<Target>();

        armHomes = new List<Target>();
        armTargets = new List<Target>();
    }

    public void addTarget(Target t)
    {
        if (t.relevantObj == Target.ObjType.Leg && t.type == Target.TargetType.home)
            legHomes.Add(t);
        if (t.relevantObj == Target.ObjType.Leg && t.type == Target.TargetType.target)
            legTargets.Add(t);
        if (t.relevantObj == Target.ObjType.Arm && t.type == Target.TargetType.home)
            armHomes.Add(t);
        if (t.relevantObj == Target.ObjType.Arm && t.type == Target.TargetType.target)
            armTargets.Add(t);
    }

    public void removeTarget(Target t)
    {
        if (t.relevantObj == Target.ObjType.Leg && t.type == Target.TargetType.home)
            legHomes.Remove(t);
        if (t.relevantObj == Target.ObjType.Leg && t.type == Target.TargetType.target)
            legTargets.Remove(t);
        if (t.relevantObj == Target.ObjType.Arm && t.type == Target.TargetType.home)
            armHomes.Remove(t);
        if (t.relevantObj == Target.ObjType.Arm && t.type == Target.TargetType.target)
            armTargets.Remove(t);
    }
}
