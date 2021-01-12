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
    protected readonly float baseSpeed = 5.0f;
    // protected readonly float baseSpeed = 1.0f;
    protected readonly int baseHealth = 30;

    // Start is called before the first frame update
    void Start() {
        currentHealth = baseHealth;
        isChasingPlayer = false;
        target = Game.getPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            path = BoardManager.getPath(transform.position, target.transform.position);
            currentWaypoint = path[1];
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, baseSpeed * Time.deltaTime);
            if ((target.transform.position - transform.position).magnitude <= 0.5f) {
                target.dealDamage(1);
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
