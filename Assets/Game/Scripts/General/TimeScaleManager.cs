using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TimeScaleManager : MonoBehaviour {
    public enum RequestPriority {
        PAUSE_MENU,
        HITSTUN
    }
    
    static TimeScaleManager instance;
    public static TimeScaleManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<TimeScaleManager>();
                if (instance == null) {
                    GameObject newGameObject = new GameObject("TimeScaleManager");
                    instance = newGameObject.AddComponent<TimeScaleManager>();
                    Debug.LogWarning("TimeScaleManager not found in scene. Created new instance at runtime.");
                }
            }
            return instance;
        }
    }

    [SerializeField]private List<TimeScaleRequest> timeScaleRequests = new List<TimeScaleRequest>();
    
    
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    [Serializable]
    public class TimeScaleRequest {
        /// <summary>
        /// 0 is highest priority. Higher numbers are lower priority.
        /// </summary>
        public RequestPriority priority;
        public float timeScale;
    }


    private void UpdateTimeScale() {
        TimeScaleRequest request = GetHighestPriorityRequest();
        if (request != null) {
            Time.timeScale = request.timeScale;
        } else {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Lower priority number is higher priority.
    /// </summary>
    /// <returns></returns>
    private TimeScaleRequest GetHighestPriorityRequest() {
        TimeScaleRequest highestPriorityRequest = null;
        foreach (TimeScaleRequest request in timeScaleRequests) {
            if (highestPriorityRequest == null || request.priority < highestPriorityRequest.priority) {
                highestPriorityRequest = request;
            }
        }
        return highestPriorityRequest;
    }



    public void AddRequest(RequestPriority priority, float timeScale) {
        TimeScaleRequest request = new TimeScaleRequest();
        request.priority = priority;
        request.timeScale = timeScale;
        timeScaleRequests.Add(request);
        UpdateTimeScale();
    }
    public void EndRequest(RequestPriority priority) {
        TimeScaleRequest request = timeScaleRequests.Find(x => x.priority == priority);
        if (request != null) {
            timeScaleRequests.Remove(request);
            UpdateTimeScale();
        }
    }
    
    public void UpdateRequest(RequestPriority priority, float timeScale) {
        TimeScaleRequest request = timeScaleRequests.Find(x => x.priority == priority);
        if (request != null) {
            request.timeScale = timeScale;
            UpdateTimeScale();
        }
    }

    private void OnDestroy() {
        Time.timeScale = 1;
    }
}
