using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Holdable {
    protected const int GUN_IGNORE_LAYER = ~(1 << 8);
    protected readonly string name;
    protected readonly float fireCooldown;
    protected readonly int damage;
    protected readonly float range;
    protected readonly int maxAmmo;
    protected float timeLastShot;
    protected int ammoRemaining;
    protected float bulletSpread;
    protected float stoppingPower;

    public Weapon(string name,
                    float fireCooldown, 
                    int damage, 
                    float range,
                    int maxAmmo, 
                    float bulletSpread, 
                    float stoppingPower) {
        this.name = name;
        this.fireCooldown = fireCooldown;
        this.damage = damage;
        this.range = range;
        this.maxAmmo = maxAmmo;
        this.timeLastShot = this.fireCooldown * -1;
        this.ammoRemaining = this.maxAmmo;
        this.bulletSpread = bulletSpread;
        this.stoppingPower = stoppingPower;
    }

    public override string getName() {
        return this.name;
    }

    public override void primaryUsed(Vector3 objectSource, Vector3 mathSource, Vector3 direction) {
        Vector3 directionWithSpray = this.addSpray(mathSource, direction);
        if (this.gunIsReady()) {
            timeLastShot = Time.time;
            this.fireWeapon(objectSource, mathSource, directionWithSpray);
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

    protected abstract void fireWeapon(Vector3 objectSource, Vector3 mathSource, Vector3 direction);

    protected void shootBullet(Vector3 objectSource, Vector3 mathSource, Vector3 direction) {
        RaycastHit hit;
        Vector3 tracerEnd;
        if (Physics.Raycast(mathSource, direction, out hit, Mathf.Infinity, GUN_IGNORE_LAYER)) {
            GameObject objectHit = hit.collider.gameObject;
            if (this.range >= hit.distance) {
                if (isMover(objectHit)) {
                    Mover mover = objectHit.GetComponents(typeof(Mover))[0] as Mover;
                    mover.dealDamage(this.damage, this.stoppingPower);
                }
                tracerEnd = hit.point;
            } else {
                tracerEnd = objectSource + ((hit.point - objectSource).normalized * this.range);
            }
        } else {
            tracerEnd = objectSource + (direction * this.range);
        }
        TracerManager.createTracer(objectSource, tracerEnd);
    }

    // TODO: Normal Distribution
    // TODO: Vertical Spray
    protected Vector3 addSpray(Vector3 source, Vector3 direction) {
        float angle = Mathf.Atan2(direction.z, direction.x);
        angle += Random.Range(-this.bulletSpread, this.bulletSpread);
        float newX = Mathf.Cos(angle);
        float newZ = Mathf.Sin(angle);
        Vector3 newDirection = new Vector3(newX, direction.y, newZ);
        return newDirection.normalized;
    }

    protected bool isMover(GameObject gameObject) {
        return gameObject.GetComponents(typeof(Mover)).Length > 0;
    }
}