using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    protected const int IGNORE_LAYER = ~(1 << 8);
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

    protected void moveInDirection(Vector3 direction, float maxDistance) {
        this.updateStoppingPowerApplied();
        float speed = Mathf.Min(this.getSpeed(), maxDistance);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, direction, out hit, speed * Time.deltaTime, IGNORE_LAYER)) {
            transform.position = hit.point + (hit.normal * 0.51f);
        } else {
            transform.position += direction.normalized * speed * Time.deltaTime;
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
        return (speedWithStoppingPower > minimumPossibleSpeed ? speedWithStoppingPower : minimumPossibleSpeed);
    }
}
