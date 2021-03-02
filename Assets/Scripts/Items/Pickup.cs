using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {
    private Item item;
    private GameObject itemAvatar;

    public void setItem(Item item) {
        this.item = item;
        this.itemAvatar = Instantiate(item.prefab, this.transform.position, Quaternion.identity);
        this.item.pickupTransform.applyTransformation(this.itemAvatar);
        this.itemAvatar.transform.SetParent(this.transform);
    }

    public Item getPickup() {
        return this.item;
    }
}
