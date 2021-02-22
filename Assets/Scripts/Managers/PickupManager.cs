using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {
    public Item riflePrefab;
    public GameObject pickupPrefab;
    private const float pickupSpawnTime = 5.0f;
    private static PickupManager pm;

    float lastPickupTime;
    
    void Awake() {
        if (pm != null) {
            Debug.LogWarning("Static pm object already found! Deleting and making another");
            GameObject.Destroy(pm);
        }
        pm = this;
        pm.lastPickupTime = 0.0f;
    }

    void Update() {
        this.lastPickupTime += Time.deltaTime;
        if (this.lastPickupTime >= pickupSpawnTime) {
            this.lastPickupTime = this.lastPickupTime % pickupSpawnTime;
            Vector3 pickupLocation = BoardManager.getRandomLocation();
            Debug.Log(pickupLocation);
            GameObject newPickup = Instantiate(pickupPrefab, pickupLocation, Quaternion.identity);
            newPickup.GetComponent<Pickup>().setItem(ScriptableObject.Instantiate(riflePrefab));
        }
    }

    public static Item pickupItem(Pickup pickup) {
        Item item = pickup.getPickup();
        Destroy(pickup.gameObject);
        return item;
    }
}
