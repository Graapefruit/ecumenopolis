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
    public float stoppingPower;
    public float bloomPerShot;
    public float bloomRecoveryRate;
    public float minBloom;
    public float maxBloom;
    private float bloomPerShotRad;
    private float currentBloom;
    private float timeLastShot;
    private int ammoRemaining;

    void OnEnable() {
        this.timeLastShot = 0.0f;
        this.ammoRemaining = this.maxAmmo;
        this.currentBloom = this.minBloom;
        this.timeLastShot = Time.time;
    }

    public override void primaryUsed(Shooter shooter, Vector3 source, Vector3 direction) {
        if (this.gunIsReady()) {
            this.fireWeapon(shooter, source, direction.normalized);
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

    public int reloadReturnRemaining(int amount) {
        int amountUsed = Mathf.Min(amount, this.maxAmmo - this.ammoRemaining);
        this.ammoRemaining += amountUsed;
        return amount - amountUsed;
    }

    protected void fireWeapon(Shooter shooter, Vector3 source, Vector3 direction) {
        Vector3 directionWithSpray = this.manageBloom(direction);
        Vector3 tracerStart = shooter.getTracerSource();
        Vector3 tracerEnd = tracerStart + (directionWithSpray * this.range);
        RaycastHit[] objectsHit = Physics.RaycastAll(new Ray(source, directionWithSpray), this.range, GUN_IGNORE_LAYER);
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

    private Vector3 manageBloom(Vector3 direction) {
        this.currentBloom -= this.bloomRecoveryRate * (Time.time - this.timeLastShot);
        this.currentBloom = Mathf.Min(Mathf.Max(this.currentBloom, this.minBloom), this.maxBloom);
        Vector3 directionWithSpray = this.getSpray(direction);
        this.currentBloom += this.bloomPerShot;
        this.timeLastShot = Time.time;
        return directionWithSpray;
    }

    protected Vector3 getSpray(Vector3 direction) {
        Vector2 deltaPoints = GrapeMath.getRandomLocationInSphere(this.currentBloom);
        float verticalAngle = Mathf.Atan2(Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.z, 2)), direction.y) + deltaPoints.y;
        float horizontalAngle = Mathf.Atan2(direction.z, direction.x) + deltaPoints.x;
        return new Vector3(Mathf.Cos(horizontalAngle), Mathf.Cos(verticalAngle), Mathf.Sin(horizontalAngle)).normalized;
    }

    protected bool isMover(GameObject gameObject) {
        return gameObject.GetComponents(typeof(Mover)).Length > 0;
    }
}