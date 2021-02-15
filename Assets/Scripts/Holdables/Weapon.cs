﻿using System;
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

    public override void primaryUsed(Shooter shooter, Vector3 source, Vector3 direction) {
        Vector3 directionWithSpray = this.addSpray(source, direction);
        if (this.gunIsReady()) {
            timeLastShot = Time.time;
            this.fireWeapon(shooter, source, directionWithSpray);
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

    protected abstract void fireWeapon(Shooter shooter, Vector3 source, Vector3 direction);

    protected void shootBullet(Shooter shooter, Vector3 source, Vector3 direction) {
        Vector3 tracerStart = shooter.getTracerSource();
        Vector3 tracerEnd = tracerStart + (direction * this.range);
        RaycastHit[] objectsHit = Physics.RaycastAll(new Ray(source, direction), this.range, GUN_IGNORE_LAYER);
        Array.Sort<RaycastHit>(objectsHit, new Comparison<RaycastHit>((i1, i2) => i2.distance.CompareTo(i1.distance)));
        // Because RaycastHit is not nullable and has no public constructor, modularizing the code without making a container class is difficult
        RaycastHit hit;
        if (objectsHit.Length > 0) {
            hit = objectsHit[0];
            Debug.Log(hit);
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