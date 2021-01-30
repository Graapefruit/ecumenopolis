using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    private const int IGNORE_INTANGIBLE = ~(1 << 8);
    private bool isChasingPlayer;
    private PlayerCharacter target;
    private const float detectionRange = 7.0f;
    private List<Vector3> path;
    private Vector3 currentWaypoint;
    private float pathRefreshCooldown;
    private Weapon bite;

    public Enemy() : base(30, 3.0f) {
        this.pathRefreshCooldown = 0.0f;
        this.bite = new DretchBite();
    }

    public void giveTarget(PlayerCharacter target) {
        this.target = target;
    }

    // Update is called once per frame
    void Update() {
        if (target) {
            if (this.canSeeTarget()) {
                this.moveTowardsDestination(this.target.transform.position);
                if ((target.transform.position - transform.position).magnitude <= 1.1f) {
                    Vector3 direction = (transform.position - this.target.transform.position).normalized;
                    this.bite.primaryUsed(transform.position, this.target.transform.position);
                }
                this.pathRefreshCooldown = 0.0f;
            } else {
                Debug.Log(this.pathRefreshCooldown);
                if (this.pathRefreshCooldown <= 0.0f) {
                    this.generateNewPath();
                }
                
                bool arrivedAtWaypoint = this.moveTowardsDestination(currentWaypoint);
                if (arrivedAtWaypoint) {
                    if (this.path.Count > 0) {
                        this.getNextWaypoint();
                    } else {
                        this.generateNewPath();
                    }
                }
                this.pathRefreshCooldown -= Time.deltaTime;
            }
        }
    }

    private bool canSeeTarget() {
        Vector3 source = transform.position;
        Vector3 direction = (this.target.transform.position - source).normalized;
        float distance = (this.target.transform.position - source).magnitude;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance, IGNORE_INTANGIBLE)) {
            return hit.collider.gameObject.GetComponent<PlayerCharacter>() == this.target;
        }
        return false;
    }

    private void getNextWaypoint() {
        this.currentWaypoint = path[0];
        this.path.RemoveAt(0);
    }

    private void generateNewPath() {
        this.path = BoardManager.getPath(transform.position, target.transform.position);
        this.getNextWaypoint();
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        distanceToTarget = distanceToTarget / 4;
        // TODO: Exponential function: closer = faster refrshes, further = longer ones
        this.pathRefreshCooldown = distanceToTarget * 0.15f;
    } 
}
