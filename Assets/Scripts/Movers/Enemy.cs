using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    private const int IGNORE_INTANGIBLE = ~(1 << 8);
    private bool isChasingPlayer;
    private PlayerCharacter target;
    private const float detectionRange = 7.0f;
    private Vector3 currentWaypoint;
    private Weapon bite;

    public override void Awake() {
        base.Awake();
        base.setup(30, 3.0f);
        this.bite = new DretchBite();
    }

    public void Start() {
        this.currentWaypoint = this.getNextWaypoint(target.transform.position);
    }

    public void giveTarget(PlayerCharacter target) {
        this.target = target;
    }

    // Update is called once per frame
    void Update() {
        if (target) {
            bool getNewWaypoint = false;
            if (this.canSeeTarget()) {
                this.moveTowardsDestination(this.target.transform.position);
                if ((target.transform.position - transform.position).magnitude <= 1.1f) {
                    Vector3 direction = (this.target.transform.position - transform.position).normalized;
                    this.bite.primaryUsed(transform.position, this.target.transform.position);
                }
                getNewWaypoint = true;
            } else {
                if (getNewWaypoint || this.arrivedAtWaypoint()) {
                    this.currentWaypoint = this.getNextWaypoint(target.transform.position);
                }
                this.moveTowardsDestination(currentWaypoint);
            }
        }
    }

    private bool canSeeTarget() {
        Vector3 source = transform.position;
        Vector3 direction = (this.target.transform.position - source).normalized;
        float distance = (this.target.transform.position - source).magnitude;
        RaycastHit hit;
        if (Physics.Raycast(source, direction, out hit, distance, IGNORE_INTANGIBLE)) {
            return hit.collider.gameObject.GetComponent<PlayerCharacter>() == this.target;
        }
        return false;
    }

    private bool arrivedAtWaypoint() {
        return (transform.position - this.currentWaypoint).magnitude < 0.05f;
    }
}
