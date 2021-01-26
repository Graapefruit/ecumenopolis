using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Holdable {
    protected readonly float fireCooldown;
    protected readonly int damage;
    protected readonly int maxAmmo;
    protected float timeLastShot;
    protected int ammoRemaining;
    protected float bulletSpread;
    protected float stoppingPower;

    public Weapon(float fc, int d, int maxAmmo, float bs, float sp) {
        this.fireCooldown = fc;
        this.damage = d;
        this.maxAmmo = maxAmmo;
        this.timeLastShot = this.fireCooldown * -1;
        this.ammoRemaining = this.maxAmmo;
        this.bulletSpread = bs;
    }

    public override void primaryUsed(Vector3 source, Vector3 destination) {
        Vector3 direction = this.getFireDirection(source, destination);
        if (this.gunIsReady()) {
            timeLastShot = Time.time;
            this.fireWeapon(source, direction);
            this.ammoRemaining--;
        }
    }

    public virtual void refillAmmo(int amount) {
        this.ammoRemaining = Mathf.Min(maxAmmo, amount + this.ammoRemaining);
    }

    public virtual int getRemainingAmmo() {
        return this.ammoRemaining;
    }

    private bool gunIsReady() {
        return Time.time - timeLastShot >= fireCooldown && ammoRemaining > 0;
    }

    protected abstract void fireWeapon(Vector3 source, Vector3 direction);

    // protected virtual void fireGun(Vector3 source, Vector3 direction) {
    //     RaycastHit hit;
    //     Vector3 tracerEnd;
    //     // TODO: Tracer if the bullet goes off the map too
    //     if (Physics.Raycast(source, direction, out hit, Mathf.Infinity)) {
    //         GameObject objectHit = hit.collider.gameObject;
    //         if (isMover(objectHit)) {
    //             Mover mover = objectHit.GetComponents(typeof(Mover))[0] as Mover;
    //             mover.dealDamage(damage);
    //         }
    //         tracerEnd = hit.point;
    //     } else {
    //         tracerEnd = source + (direction * 15.0f);//??
    //     }
    //     TracerManager.createTracer(source, tracerEnd);
    // }

    // TODO: Normal Distribution
    // TODO: Check for 0
    protected Vector3 getFireDirection(Vector3 source, Vector3 destination) {
        Vector3 initialDirection = (destination - source).normalized;
        float angle = Mathf.Atan(initialDirection.z / initialDirection.x);
        angle += Random.Range(-this.bulletSpread, this.bulletSpread);
        float newX = Mathf.Cos(angle) * (source.x < destination.x ? 1 : -1);
        float newZ = Mathf.Sin(angle) * (source.x < destination.x ? 1 : -1);
        Vector3 newDirection = new Vector3(newX, 0.0f, newZ);
        return newDirection.normalized;
    }

    protected bool isMover(GameObject gameObject) {
        return gameObject.GetComponents(typeof(Mover)).Length > 0;
    }
}