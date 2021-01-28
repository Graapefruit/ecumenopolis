using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    protected const int COLLISION_LAYERS = (1 << 9) + (1 << 10);
    protected const float stoppingPowerRecoveryRatePerSecond = 1.5f;
    protected const float minimumPossibleSpeed = 0.1f;
    protected readonly float baseSpeed;
    protected readonly int baseHealth;
    protected float stoppingPowerApplied;
    protected float stoppingPowerLastUpdate;
    protected int currentHealth;

    public Mover(int baseHealth, float baseSpeed) {
        this.baseHealth = baseHealth;
        this.baseSpeed = baseSpeed;
        this.stoppingPowerApplied = 0;
        this.currentHealth = this.baseHealth;
    }

    public virtual void dealDamage(int damageDealt, float stoppingPower) {
        this.currentHealth -= damageDealt;
        if (this.currentHealth <= 0) {
            Destroy(gameObject);
        }
        this.stoppingPowerApplied = stoppingPower;
        this.stoppingPowerLastUpdate = Time.time;
    }

    // TODO; Remove boilerplate in two functions below: both are similar
    protected bool moveTowardsDestination(Vector3 destination) {
        this.updateStoppingPowerApplied();
        float speed = this.getSpeed();
        Vector3 direction = (destination - transform.position).normalized;
        float distance = (destination - transform.position).magnitude;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.4f, direction, out hit, speed, COLLISION_LAYERS)) {
            transform.position = hit.point + (hit.normal * 0.5f);
        } else {
            if (distance < speed) {
                transform.position = destination;
                return true;
            } else {
                transform.position += direction.normalized * speed;
            }
        }
        return false;
    }

    protected void moveInDirection(Vector3 direction) {
        this.updateStoppingPowerApplied();
        float speed = this.getSpeed();
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, direction, out hit, speed, COLLISION_LAYERS)) {
            transform.position = hit.point + (hit.normal * 0.51f);
        } else {
            transform.position += direction.normalized * speed;
        }
    }

    private void updateStoppingPowerApplied() {
        float stoppingPowerReduction = (Time.time - this.stoppingPowerLastUpdate) * stoppingPowerRecoveryRatePerSecond;
        if (stoppingPowerReduction < this.stoppingPowerApplied) {
            this.stoppingPowerApplied -= stoppingPowerReduction;
        } else {
            this.stoppingPowerApplied = 0;
        }
        this.stoppingPowerLastUpdate = Time.time;
    }

    private float getSpeed() {
        float speedWithStoppingPower = this.baseSpeed - this.stoppingPowerApplied;
        return (speedWithStoppingPower > minimumPossibleSpeed ? speedWithStoppingPower : minimumPossibleSpeed) * Time.deltaTime;
    }
}
