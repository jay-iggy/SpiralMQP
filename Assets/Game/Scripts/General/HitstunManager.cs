using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstunManager : MonoBehaviour {
    private static HitstunManager _instance;
    public static HitstunManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<HitstunManager>();
            }
            if(_instance == null) {
                Debug.LogError($"HitstunManager not found in scene {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            }

            return _instance;
        }
    }


    public void SetHitstun(float duration, float newTimeScale) {
        StartCoroutine(Hitstun(duration, newTimeScale));
    }
    
    private IEnumerator Hitstun(float duration, float newTimeScale) {
        TimeScaleManager.Instance.AddRequest(TimeScaleManager.RequestPriority.HITSTUN, newTimeScale);
        yield return new WaitForSecondsRealtime(duration);
        TimeScaleManager.Instance.EndRequest(TimeScaleManager.RequestPriority.HITSTUN);
    }
}
