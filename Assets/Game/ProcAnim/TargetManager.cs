using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelevantTargets
{
    public string characterName;

    public List<Target> legHomes;
    public List<Target> legTargets;

    public List<Target> armHomes;
    public List<Target> armTargets;

    public RelevantTargets(string name)
    {
        this.characterName = name;

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

public class TargetManager : MonoBehaviour
{
    public static TargetManager instance;

    public Dictionary<string, RelevantTargets> characterTargets;

    void Awake()
    {
        instance = this;
        characterTargets = new Dictionary<string, RelevantTargets>();
    }

    public void addTarget(Target t)
    {
        if (!characterTargets.ContainsKey(t.relevantCharacter))
        {
            // there is no RelevantTargets for character
            RelevantTargets rts = new RelevantTargets(t.relevantCharacter);
            rts.addTarget(t);
            characterTargets.Add(t.relevantCharacter, rts);
        }
        else
        {
            characterTargets[t.relevantCharacter].addTarget(t);
        }
    }
    public void removeTarget(Target t)
    {
        if (!characterTargets.ContainsKey(t.relevantCharacter))
        {
            // there is no RelevantTargets for character
            Debug.Log("Target to be removed has no associated RelevantTargets object");
        }
        else
        {
            characterTargets[t.relevantCharacter].removeTarget(t);
        }
    }
}
