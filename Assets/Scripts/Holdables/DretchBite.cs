using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DretchBite : Weapon {
    public DretchBite() : base("Dretch Bite", 0.5f, 8, 2.0f, int.MaxValue, 0.0f, 2.5f) {}
    protected override void fireWeapon(Vector3 objectSource, Vector3 mathSource, Vector3 direction) {
        this.shootBullet(objectSource, mathSource, direction);
    }
}
