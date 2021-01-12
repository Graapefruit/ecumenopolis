using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapChunk : Collectable
{
    private int scrapCount;

    void Start() {
        scrapCount = Random.Range(1, 10);
    }

    protected override bool validPickupCandidate(GameObject gameObject) {
        return this.gameObjectIsOfClass<PlayerCharacter>(gameObject);
    }

    protected override void getPickedUpBy(GameObject gameObject) {
        this.getClassFromGameObject<PlayerCharacter>(gameObject).pickupScrap(scrapCount);
        Destroy(this.gameObject);
    }
}
