﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon {
    public Rifle() : base("Rifle", 0.125f, 7, 5.0f, 30, Mathf.PI / 20, 1.2f) {}
    protected override void fireWeapon(Vector3 source, Vector3 direction) {
        this.shootBullet(source, direction);
    }
}
