using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
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
        if (target && this.pathRefreshCooldown <= 0.0f) {
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

        if ((target.transform.position - transform.position).magnitude <= 1.1f) {
            this.bite.primaryUsed(transform.position, target.transform.position);
        }
        this.pathRefreshCooldown -= Time.deltaTime;
    }

    private void getNextWaypoint() {
        this.currentWaypoint = path[0];
        this.path.RemoveAt(0);
    }

    private void generateNewPath() {
        this.path = BoardManager.getPath(transform.position, target.transform.position);
        this.path.RemoveAt(0);
        this.getNextWaypoint();
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        distanceToTarget = distanceToTarget / 4;
        // TODO: Exponential function: closer = faster refrshes, further = longer ones
        this.pathRefreshCooldown = distanceToTarget * 0.15f;
    } 
}
