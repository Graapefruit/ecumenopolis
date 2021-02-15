using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DretchBite : Weapon {
    public DretchBite() : base("Dretch Bite", 0.5f, 8, 2.0f, int.MaxValue, 0.0f, 2.5f) {}
    protected override void fireWeapon(Shooter shooter, Vector3 source, Vector3 direction) {
        this.shootBullet(shooter, source, direction);
    }
}
