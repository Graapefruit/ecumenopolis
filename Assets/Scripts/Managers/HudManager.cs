using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour {
    public GameObject playerHudPrefab;
    public GameObject inventoryHudPrefab;
    public GameObject hotbarHudPrefab;
    private static HudManager hm;
    
    void Awake() {
        if (hm != null) {
            Debug.LogWarning("Static hm object already found! Deleting and making another");
            GameObject.Destroy(hm);
        }
        hm = this;
    }

    public static PlayerHud getNewPlayerHudInstance() {
        return Instantiate(hm.playerHudPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerHud>();
    }

    public static InventoryHudPanel getNewInventoryHudInstance() {
        return Instantiate(hm.inventoryHudPrefab, Vector3.zero, Quaternion.identity).GetComponent<InventoryHudPanel>();
    }

    public static HotbarHudPanel getNewHotbarHudInstance() {
        return Instantiate(hm.hotbarHudPrefab, Vector3.zero, Quaternion.identity).GetComponent<HotbarHudPanel>();
    }
}
