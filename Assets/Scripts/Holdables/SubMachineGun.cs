using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMachineGun : Weapon {
    public SubMachineGun() : base("SMG", 0.085f, 5, 5.0f, 45, Mathf.PI / 35, 0.5f) {}
    
    protected override void fireWeapon(Vector3 source, Vector3 direction) {
        this.shootBullet(source, direction);
    }
}
