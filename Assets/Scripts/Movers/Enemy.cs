using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Mover, Shooter
{
    private const int IGNORE_INTANGIBLE = ~(1 << 8);
    private bool isChasingPlayer;
    private PlayerCharacter target;
    private Gun bite;
    private NavMeshAgent navMeshAgent;

    public override void Awake() {
        base.Awake();
        base.setup(30, 3.0f);
        // this.bite = new DretchBite();
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    public void giveTarget(PlayerCharacter target) {
        this.target = target;
    }

    public bool isSelf(GameObject gameObject) {
        return gameObject == this.gameObject;
    }

    public Vector3 getTracerSource() {
        return this.transform.position;
    }

    void Update() {
        if (target) {
            if ((target.transform.position - transform.position).magnitude <= 1.1f) {
                Vector3 direction = (this.target.transform.position - transform.position).normalized;
                this.bite.primaryUsed(this as Shooter, transform.position, this.target.transform.position);
            } else {
                this.setAsMoving();
                this.navMeshAgent.destination = this.target.transform.position;
            }
        } else {
            this.setAsIdle();
            // this.navMeshAgent.ResetPath(this.target.transform.position);
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
}
