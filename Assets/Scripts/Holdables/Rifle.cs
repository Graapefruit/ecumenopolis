using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon {
    protected const string name = "Rifle";
    public Rifle() : base(0.15f, 4, 4, Mathf.PI / 20, 2.0f) {}
    public override string getName() {
        return name;
    }
}
