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
    private Rigidbody rigidBody;
    private Vector3 movementDirection;
    private Vector3 positionOnFixedUpdate;
    protected Animator animator;

    public virtual void Awake() {
        this.rigidBody = this.GetComponent<Rigidbody>();
        this.movementDirection = Vector3.zero;
        this.animator = this.GetComponent<Animator>();
    }

    private void FixedUpdate() {
        this.updateMovingAnimation();
        rigidBody.MovePosition(transform.position + (movementDirection * this.getSpeed() * Time.fixedDeltaTime));
        this.movementDirection = Vector3.zero;
    }

    protected virtual void updateMovingAnimation() {
        if (this.movementDirection == Vector3.zero) {
            this.animator.SetBool("isWalking", false);
        } else {
            this.animator.SetBool("isWalking", true);
        }
    }

    public virtual void setup(int baseHealth, float baseSpeed) {
        this.baseHealth = baseHealth;
        this.baseSpeed = baseSpeed;
        this.stoppingPowerApplied = 0;
        this.currentHealth = this.baseHealth;
    }

    public virtual void dealDamage(int damageDealt, float stoppingPower) {
        this.currentHealth -= damageDealt;
        if (this.currentHealth <= 0) {
            this.initiateDeath();
        }
        this.stoppingPowerApplied = stoppingPower;
        this.stoppingPowerLastUpdate = Time.time;
    }

    protected virtual void initiateDeath() {
        Destroy(gameObject);
    }

    public void moveInDirection(Vector3 direction) {
        this.updateStoppingPowerApplied();
        this.movementDirection = direction;
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
