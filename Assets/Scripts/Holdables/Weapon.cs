using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Holdable {
    protected readonly float fireCooldown;
    protected readonly int damage;
    protected readonly int baseAmmo;
    protected float timeLastShot;
    protected int ammoRemaining;
    protected float bulletSpread;
    protected float stoppingPower;

    public Weapon(float fc, int d, int ba, float bs, float sp) {
        this.fireCooldown = fc;
        this.damage = d;
        this.baseAmmo = ba;
        this.timeLastShot = this.fireCooldown * -1;
        this.ammoRemaining = this.baseAmmo;
        this.bulletSpread = bs;
    }

    public override void primaryUsed(Vector3 source, Vector3 destination) {
        Vector3 direction = this.getFireDirection(source, destination);
        if (this.gunIsReady()) {
            this.fireGun(source, direction);
        }
    }

    public virtual void refillAmmo(int amount) {
        this.ammoRemaining = Mathf.Min(baseAmmo, amount + this.ammoRemaining);
    }

    public virtual int getRemainingAmmo() {
        return this.ammoRemaining;
    }

    private bool gunIsReady() {
        return Time.time - timeLastShot >= fireCooldown && ammoRemaining > 0;
    }

    private void fireGun(Vector3 source, Vector3 direction) {
        // this.ammoRemaining--;
        RaycastHit hit;
        // TODO: Tracer if the bullet goes off the map too
        if (Physics.Raycast(source, direction, out hit, Mathf.Infinity)) {
            GameObject objectHit = hit.collider.gameObject;
            if (isMover(objectHit)) {
                Mover mover = objectHit.GetComponents(typeof(Mover))[0] as Mover;
                mover.dealDamage(damage);
            }
            TracerManager.createTracer(source, hit.point);
        }
        timeLastShot = Time.time;
    }

    // TODO: Normal Distribution
    // TODO: Check for 0
    private Vector3 getFireDirection(Vector3 source, Vector3 destination) {
        Vector3 initialDirection = (destination - source).normalized;
        float angle = Mathf.Atan(initialDirection.z / initialDirection.x);
        angle += Random.Range(-this.bulletSpread, this.bulletSpread);
        Debug.Log(angle);
        float newX = Mathf.Cos(angle) * (source.x < destination.x ? 1 : -1);
        float newZ = Mathf.Sin(angle) * (source.x < destination.x ? 1 : -1);
        Vector3 newDirection = new Vector3(newX, 0.0f, newZ);
        return newDirection.normalized;
    }

    private bool isMover(GameObject gameObject) {
        return gameObject.GetComponents(typeof(Mover)).Length > 0;
    }
}