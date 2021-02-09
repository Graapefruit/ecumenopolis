using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    protected const int COLLISION_LAYERS = (1 << 9) + (1 << 10);
    protected const float stoppingPowerRecoveryRatePerSecond = 1.5f;
    protected const float minimumPossibleSpeed = 0.1f;
    protected float baseSpeed;
    protected int baseHealth;
    protected float stoppingPowerApplied;
    protected float stoppingPowerLastUpdate;
    protected int currentHealth;
    private Rigidbody rigidBody;
    private Vector3 movementDirection;
    private bool movingTowardsWaypoint;
    private Vector3 positionOnFixedUpdate;

    public virtual void Awake() {
        this.rigidBody = this.GetComponent<Rigidbody>();
        this.movementDirection = Vector3.zero;
        this.movingTowardsWaypoint = false;
    }

    private void FixedUpdate() {
        if (movingTowardsWaypoint) {
            Vector3 waypointDirection = positionOnFixedUpdate - transform.position;
            float speedThisTick = this.getSpeed() * Time.fixedDeltaTime;
            if (speedThisTick >= waypointDirection.magnitude) {
                rigidBody.MovePosition(positionOnFixedUpdate);
                movingTowardsWaypoint = false;
            } else {
                rigidBody.MovePosition(transform.position + (waypointDirection.normalized * speedThisTick));
            }
        } else {
            rigidBody.MovePosition(transform.position + (movementDirection * this.getSpeed() * Time.fixedDeltaTime));
            this.movementDirection = Vector3.zero;
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
            Destroy(gameObject);
        }
        this.stoppingPowerApplied = stoppingPower;
        this.stoppingPowerLastUpdate = Time.time;
    }

    protected void moveInDirection(Vector3 direction) {
        this.updateStoppingPowerApplied();
        this.movementDirection = direction;
    }

    protected void moveTowardsDestination(Vector3 destination) {
        this.moveInDirection((destination - transform.position).normalized);
        this.positionOnFixedUpdate = destination;
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
