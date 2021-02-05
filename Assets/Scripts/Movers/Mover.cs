using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    private const int TERRAIN_LAYER_MASK = 1 << 9;
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

    protected Vector3 getNextWaypoint(Vector3 destination) {
        RaycastHit hit;
        Vector3 direction = (destination - transform.position).normalized;
        float distance = (destination - transform.position).magnitude;
        while (Physics.SphereCast(transform.position, 0.45f, direction, out hit, distance, TERRAIN_LAYER_MASK)) {
            Vector3 leftEdge, rightEdge;
            Vector3 newDestination;
            this.getWallEdges(hit.collider.gameObject, out leftEdge, out rightEdge);
            // TODO: Randomize so monsters spread out
            if ((destination - leftEdge).magnitude <= (destination - rightEdge).magnitude) {
                newDestination = leftEdge;
            } else {
                newDestination = rightEdge;
            }
            // Avoids infinite loops: happens when a ball gets stuck on an edge that spherecasting can't go through
            if (newDestination == destination) {
                break;
            }
            destination = newDestination;
            direction = (destination - transform.position).normalized;
            distance = (destination - transform.position).magnitude;
        }
        return destination;
    }

    private void getWallEdges(GameObject wall, out Vector3 leftEdge, out Vector3 rightEdge) {
        Vector3 location = transform.position;
        float xpos = wall.transform.position.x;
        float zpos = wall.transform.position.z;
        float xscale = wall.transform.localScale.x;
        float zscale = wall.transform.localScale.z;
        float size = 0.485f;
        float top = zpos + (zscale / 2.0f);
        float leftSide = xpos - (xscale / 2.0f);
        float rightSide = xpos + (xscale / 2.0f);
        float bottom = zpos - (zscale / 2.0f);
        Vector3 topLeft = new Vector3(leftSide - size, 0.5f, top + size);
        Vector3 topRight = new Vector3(rightSide + size, 0.5f, top + size);
        Vector3 bottomLeft = new Vector3(leftSide - size, 0.5f, bottom - size);
        Vector3 bottomRight = new Vector3(rightSide + size, 0.5f, bottom - size);
        // Edges
        if (location.x <= leftSide && location.z >= top) {
            leftEdge = topRight;
            rightEdge = bottomLeft;
        } else if (location.x >= rightSide && location.z >= top) {
            leftEdge = bottomRight;
            rightEdge = topLeft;
        } else if (location.x <= leftSide && location.z <= bottom) {
            leftEdge = topLeft;
            rightEdge = bottomRight;
        } else if (location.x >= rightSide && location.z <= bottom) {
            leftEdge = bottomLeft;
            rightEdge = topRight;
        // Faces
        } else if (location.z >= top) {
            leftEdge = topRight;
            rightEdge = topLeft;
        } else if (location.z <= bottom) {
            leftEdge = bottomLeft;
            rightEdge = bottomRight;
        } else if (location.x <= leftSide) {
            leftEdge = topLeft;
            rightEdge = bottomLeft;
        } else if (location.x >= rightSide) {
            leftEdge = bottomRight;
            rightEdge = topRight;
        } else {
            Debug.LogFormat("Help! I have no idea where to go on this object!\nLocation: {0}\nTop Left: {1}\nTop Right: {2}\nBottom Left: {3}\nBottom Right: {4}", location, topLeft, topRight, bottomLeft, bottomRight);
            leftEdge = transform.position;
            rightEdge = transform.position;
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
