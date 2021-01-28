using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
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

    public void dealDamage(int damageDealt, float stoppingPower) {
        this.currentHealth -= damageDealt;
        if (this.currentHealth <= 0) {
            Destroy(gameObject);
        }
        this.stoppingPowerApplied = stoppingPower;
        this.stoppingPowerLastUpdate = Time.time;
    }

    protected void moveTowardsLocation(Vector3 destination) {
        this.updateStoppingPowerApplied();
        float speed = this.getSpeed();
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    protected void moveInDirection(Vector3 direction) {
        this.updateStoppingPowerApplied();
        float speed = this.getSpeed();
        Debug.LogFormat("speed: {0}, base speed: {1}", speed, this.baseSpeed);
        transform.position += direction.normalized * speed * Time.deltaTime;
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
        return (speedWithStoppingPower > minimumPossibleSpeed ? speedWithStoppingPower : minimumPossibleSpeed);
    }
}
