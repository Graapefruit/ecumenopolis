using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Mover
{
    private const int IGNORE_INTANGIBLE = ~(1 << 8);
    private bool isChasingPlayer;
    private PlayerCharacter target;
    private Weapon bite;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    public override void Awake() {
        base.Awake();
        base.setup(30, 3.0f);
        this.bite = new DretchBite();
        this.animator = this.GetComponent<Animator>();
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    public void giveTarget(PlayerCharacter target) {
        this.target = target;
    }

    void Update() {
        if (target) {
            this.animator.SetBool("isMoving", true);
            if ((target.transform.position - transform.position).magnitude <= 1.1f) {
                Vector3 direction = (this.target.transform.position - transform.position).normalized;
                this.bite.primaryUsed(transform.position, transform.position, this.target.transform.position);
            } else {
                this.navMeshAgent.destination = this.target.transform.position;
            }
        } else {
            this.animator.SetBool("isMoving", false);
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
