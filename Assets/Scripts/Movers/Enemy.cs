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

    public Enemy() : base(30, 3.0f) {}

    public void giveTarget(PlayerCharacter target) {
        this.target = target;
    }

    // Update is called once per frame
    void Update() {
        if (target) {
            path = BoardManager.getPath(transform.position, target.transform.position);
            currentWaypoint = path[1];
            Vector3 direction = (currentWaypoint - transform.position).normalized;
            float distance = (currentWaypoint - transform.position).magnitude;
            this.moveInDirection(direction, distance);
            if ((target.transform.position - transform.position).magnitude <= 0.5f) {
                target.dealDamage(1, 2.0f);
            }
        }
    }

    private bool playerInDetectionRange() {
        return (Game.getPlayer().transform.position - transform.position).magnitude <= detectionRange;
    }

    private bool creatureGrowsImpatient() {
        return (Game.getPlayer().transform.position - transform.position).magnitude > (detectionRange * 1.5);
    }
}
