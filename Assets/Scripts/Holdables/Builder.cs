using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Weapon {
    public Builder() : base("Builder", 1.05f, 0, 3.25f, 10, 0.0f, 0.0f) {}

    protected override void fireWeapon(Vector3 source, Vector3 direction) {
        float placementDistance = direction.magnitude;
        Vector3 placementLocation = source + (direction * this.range);
        BuildingManager.createCaltrops(placementLocation, Quaternion.Euler(0.0f, Mathf.Atan(direction.x / direction.z) * Mathf.Rad2Deg, 0.0f));
    }
}
