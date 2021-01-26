using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon {
    protected const string name = "Rifle";
    public Rifle() : base(0.125f, 4, 30, Mathf.PI / 20, 2.0f) {}
    public override string getName() {
        return name;
    }
    protected override void fireWeapon(Vector3 source, Vector3 direction) {
        RaycastHit hit;
        Vector3 tracerEnd;
        // TODO: Tracer if the bullet goes off the map too
        if (Physics.Raycast(source, direction, out hit, Mathf.Infinity)) {
            GameObject objectHit = hit.collider.gameObject;
            if (isMover(objectHit)) {
                Mover mover = objectHit.GetComponents(typeof(Mover))[0] as Mover;
                mover.dealDamage(damage);
            }
            tracerEnd = hit.point;
        } else {
            tracerEnd = source + (direction * 15.0f);//??
        }
        TracerManager.createTracer(source, tracerEnd);
    }
}
