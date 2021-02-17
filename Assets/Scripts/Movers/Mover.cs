using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    protected const float stoppingPowerRecoveryRatePerSecond = 1.5f;
    protected const float minimumPossibleSpeed = 0.1f;
    protected float baseSpeed;
    protected int baseHealth;
    protected float stoppingPowerApplied;
    protected float stoppingPowerLastUpdate;
    protected int currentHealth;
    private Vector3 positionOnFixedUpdate;
    protected Animator animator;

    public virtual void Awake() {
        this.animator = this.GetComponent<Animator>();
    }

    protected virtual void setAsMoving() {
        this.animator.SetBool("isWalking", true);
    }

    protected virtual void setAsIdle() {
        this.animator.SetBool("isWalking", false);
    }

    public virtual void setup(int baseHealth, float baseSpeed) {
        this.baseHealth = baseHealth;
        this.baseSpeed = baseSpeed;
        this.stoppingPowerApplied = 0;
        this.currentHealth = this.baseHealth;
    }

    public virtual void dealDamage(int damageDealt, float stoppingPower) {
        this.currentHealth -= damageDealt;
        this.currentHealth = (currentHealth < 0 ? 0 : currentHealth);
        if (this.currentHealth <= 0) {
            this.initiateDeath();
        }
        this.stoppingPowerApplied = stoppingPower;
        this.stoppingPowerLastUpdate = Time.time;
    }

    protected virtual void initiateDeath() {
        Destroy(gameObject);
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
