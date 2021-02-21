using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour {
    public GameObject inventoryHudManagerPrefab;
    private static HudManager hm;
    
    void Awake() {
        if (hm != null) {
            Debug.LogWarning("Static hm object already found! Deleting and making another");
            GameObject.Destroy(hm);
        }
        hm = this;
    }

    public static GameObject getInventoryHudManagerPrefab() {
        return hm.inventoryHudManagerPrefab;
    }
}
