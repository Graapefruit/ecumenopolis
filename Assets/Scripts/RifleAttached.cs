using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleAttached {
    private const float fireCooldown = 0.75f;
    private const int damage = 4;
    private const int baseAmmo = 4;
    private float timeLastShot;
    private int ammoRemaining;

    public RifleAttached() {
        timeLastShot = fireCooldown * -1;
        ammoRemaining = baseAmmo;
    }

    public bool gunIsReady() {
        return Time.time - timeLastShot >= fireCooldown && ammoRemaining > 0;
    }

    public void fireGun(Vector3 source, Vector3 direction) {
        if (gunIsReady()) {
            ammoRemaining--;
            RaycastHit hit;
            if (Physics.Raycast(source, direction, out hit, Mathf.Infinity)) {
                GameObject objectHit = hit.collider.gameObject;
                if (isMover(objectHit)) {
                    Mover mover = objectHit.GetComponents(typeof(Mover))[0] as Mover;
                    mover.dealDamage(damage);
                }
            }
        }
        timeLastShot = Time.time;
    }

    public int getRemainingAmmo() {
        return ammoRemaining;
    }

    public void refillAmmo(int amount) {
        this.ammoRemaining = Mathf.Min(baseAmmo, amount + this.ammoRemaining);
    }

    private bool isMover(GameObject gameObject) {
        return gameObject.GetComponents(typeof(Mover)).Length > 0;
    }
}