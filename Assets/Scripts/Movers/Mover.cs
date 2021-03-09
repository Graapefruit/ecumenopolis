using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    protected const float STOPPING_POWER_RECOVERY_RATE_PER_SECOND = 1.5f;
    protected const float MINIMUM_POSSIBLE_SPEED = 0.1f;
    protected int baseHealth;
    protected float baseSpeed;
    private Vector3 positionOnFixedUpdate;
    protected Animator animator;

    protected int currentHealth;
    protected float currentSpeed;

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
        this.currentHealth = this.baseHealth;
        this.currentSpeed = this.baseSpeed;
    }

    public virtual void dealDamage(int damageDealt, float stoppingPower) {
        this.currentHealth -= damageDealt;
        this.currentHealth = (currentHealth < 0 ? 0 : currentHealth);
        if (this.currentHealth <= 0) {
            this.initiateDeath();
        }
    }

    protected virtual void initiateDeath() {
        Destroy(gameObject);
    }

    private float getSpeed() {
        return this.currentSpeed;
    }
}
