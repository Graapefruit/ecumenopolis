using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun")]
public class Gun : Item {
    private const int GUN_IGNORE_LAYER = ~(1 << 8);
    public int damage;
    public float fireCooldown;
    public float range;
    public int maxAmmo;
    public float bulletSpread;
    public float stoppingPower;
    private float timeLastShot;
    private int ammoRemaining;

    void OnEnable() {
        this.timeLastShot = 0.0f;
        this.ammoRemaining = this.maxAmmo;
    }

    public override void primaryUsed(Shooter shooter, Vector3 source, Vector3 direction) {
        if (this.gunIsReady()) {
            timeLastShot = Time.time;
            Vector3 directionWithSpray = this.addSpray(source, direction);
            this.fireWeapon(shooter, source, directionWithSpray);
            this.ammoRemaining--;
        }
    }

    private bool gunIsReady() {
        return Time.time - timeLastShot >= fireCooldown && ammoRemaining > 0;
    }

    public void refillAmmo(int amount) {
        this.ammoRemaining = Mathf.Min(maxAmmo, amount + this.ammoRemaining);
    }

    public int getRemainingAmmo() {
        return this.ammoRemaining;
    }

    protected void fireWeapon(Shooter shooter, Vector3 source, Vector3 direction) {
        Vector3 tracerStart = shooter.getTracerSource();
        Vector3 tracerEnd = tracerStart + (direction * this.range);
        RaycastHit[] objectsHit = Physics.RaycastAll(new Ray(source, direction), this.range, GUN_IGNORE_LAYER);
        Array.Sort<RaycastHit>(objectsHit, new Comparison<RaycastHit>((i1, i2) => i2.distance.CompareTo(i1.distance)));
        // Because RaycastHit is not nullable and has no public constructor, modularizing the code without making a container class is difficult
        RaycastHit hit;
        if (objectsHit.Length > 0) {
            hit = objectsHit[0];
            if (shooter.isSelf(hit.collider.gameObject)) {
                if (objectsHit.Length > 1) {
                    tracerEnd = hit.point;
                    this.applyBulletEffects(hit);
                }
            } else {
                tracerEnd = hit.point;
                this.applyBulletEffects(hit);
            }
        }

        TracerManager.createTracer(tracerStart, tracerEnd);
    }

    private void applyBulletEffects(RaycastHit hit) {
        GameObject gameObjectHit = hit.collider.gameObject;
        if (isMover(gameObjectHit)) {
            Mover mover = gameObjectHit.GetComponents(typeof(Mover))[0] as Mover;
            mover.dealDamage(this.damage, this.stoppingPower);
        }
    }

    // TODO: Normal Distribution
    // TODO: Vertical Spray
    // TODO: Spray sucks, direction should be modified based on current orientation (x/z, y)
    protected Vector3 addSpray(Vector3 source, Vector3 direction) {
        float angle = Mathf.Atan2(direction.z, direction.x);
        angle += UnityEngine.Random.Range(-this.bulletSpread, this.bulletSpread);
        float newX = Mathf.Cos(angle);
        float newZ = Mathf.Sin(angle);
        Vector3 newDirection = new Vector3(newX, direction.y, newZ);
        return newDirection.normalized;
    }

    protected bool isMover(GameObject gameObject) {
        return gameObject.GetComponents(typeof(Mover)).Length > 0;
    }
}