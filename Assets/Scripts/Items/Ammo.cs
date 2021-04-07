using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo", menuName = "ScriptableObjects/Ammo")]
public class Ammo : Item {

    public override void primaryUsed(Shooter shooter, Vector3 source, Vector3 direction) {
        return;
    }
}
