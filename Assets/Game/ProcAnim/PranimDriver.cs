using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PranimDriver : MonoBehaviour
{
    public string characterName;

    #region legs
    [SerializeField] private LegStepper rightLeg;
    [SerializeField] private LegStepper leftLeg;
    private string lastStepped;

    IEnumerator OneLegAtATime()
    {
        // always running
        while (true && TargetManager.instance.characterTargets.ContainsKey(characterName))
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
        while (true && TargetManager.instance.characterTargets.ContainsKey(characterName))
        {
            // Check for changes in armTargets count
            if (TargetManager.instance.characterTargets[characterName].armTargets.Count > 0)
            {
                // Assign targets to arms
                foreach (var t in TargetManager.instance.characterTargets[characterName].armTargets)
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
                foreach (var t in TargetManager.instance.characterTargets[characterName].armHomes)
                {
                    if (t.rightOrLeft == "right")
                        rightArm.target = t.transform;
                    else if (t.rightOrLeft == "left")
                        leftArm.target = t.transform;
                }
            }

            // Update the last known count
            lastArmTargetsCount = TargetManager.instance.characterTargets[characterName].armTargets.Count;

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
    private Material[] _defaultMaterials;
    
    
    public void UpdateMaterialsToDefaults() {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].material = _defaultMaterials[i];
        }
    }

    public void SetAllMaterialsToOneMat(Material m) {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers) {
            r.material = m;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        lastStepped = "right";
        lastArmTargetsCount = 0;
        StartCoroutine(OneLegAtATime());
        StartCoroutine(ArmsTrackItems());
    }

    private void Awake() {
        // Store default materials
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        _defaultMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) {
            _defaultMaterials[i] = renderers[i].material;
        }
    }
}
