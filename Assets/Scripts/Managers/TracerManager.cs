using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerManager : MonoBehaviour
{
    public GameObject tracer;
    private PriorityQueueAscending<GameObject> tracers;
    private const float tracerDuration = 0.15f;
    private static TracerManager tm;
    
    void Awake() {
        if (tm != null) {
            Debug.LogWarning("Static tm object already found! Deleting and making another");
            GameObject.Destroy(tm);
        }
        tm = this;
        tm.tracers = new PriorityQueueAscending<GameObject>();
    }

    public static void createTracer(Vector3 source, Vector3 destination) {
        Vector3 location = source;
        Vector3 heading = (destination - source);
        float magnitude = heading.magnitude;
        Quaternion direction = Quaternion.LookRotation(heading / magnitude);
        Vector3 scale = new Vector3(0.0f, 0.0f, magnitude);
        GameObject newTracer = Instantiate(tm.tracer, location, direction);
        newTracer.transform.localScale = scale;
        tm.tracers.add(newTracer, Time.time + tracerDuration);
    }

    void Update() {
        while (!(this.tracers.empty()) && this.tracers.peek().second <= Time.time) {
            Destroy(this.tracers.pop());
        }
    }
}
