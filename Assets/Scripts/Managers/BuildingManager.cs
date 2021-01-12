using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject caltrops; 
    private static BuildingManager bm;
    
    void Awake() {
        if (bm != null) {
            Debug.LogWarning("Static bm object already found! Deleting and making another");
            GameObject.Destroy(bm);
        }
        bm = this;
    }

    public static void createCaltrops(Vector3 location, Quaternion rotation) {
        Instantiate(bm.caltrops, location, rotation);
    }
}
