using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Weapon {
    private const float maxPlacementDistance = 3.25f;
    protected const string name = "Builder";
    public Builder() : base(1.05f, 0, 10, 0.0f, 0.0f) {}

    public override string getName() {
        return name;
    }

    protected override void fireWeapon(Vector3 source, Vector3 direction) {
        float placementDistance = direction.magnitude;
        Vector3 placementLocation = source + (direction * maxPlacementDistance);
        BuildingManager.createCaltrops(placementLocation, Quaternion.Euler(0.0f, Mathf.Atan(direction.x / direction.z) * Mathf.Rad2Deg, 0.0f));
    }

    // public override void primaryUsed(Vector3 source, Vector3 destination) {
    //     Vector3 direction = (destination - source).normalized;
    //     float placementDistance = (destination - source).magnitude;
    //     Vector3 maxDistancePlacement = source + (direction * maxPlacementDistance);
    //     Vector3 placementLocation = (placementDistance <= maxPlacementDistance ? destination : maxDistancePlacement);
    //     BuildingManager.createCaltrops(placementLocation, Quaternion.Euler(0.0f, Mathf.Atan(direction.x / direction.z) * Mathf.Rad2Deg, 0.0f));
    // }
}
