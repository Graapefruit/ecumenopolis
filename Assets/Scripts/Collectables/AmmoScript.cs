using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : Collectable
{
    private int ammoCount;

    void Start() {
        this.ammoCount = 8 + Random.Range(0, 8);
    }

    protected override bool validPickupCandidate(GameObject gameObject) {
        return this.gameObjectIsOfClass<PlayerCharacter>(gameObject);
    }

    protected override void getPickedUpBy(GameObject gameObject) {
        this.getClassFromGameObject<PlayerCharacter>(gameObject).pickupAmmo(ammoCount);
        Destroy(this.gameObject);
    }
}
