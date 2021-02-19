using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover, Shooter
{
    public Item held;
    private Inventory inventory;
    private float xRotation;
    private Transform followTarget;
    private Vector3 moveDelta;
    private CharacterController characterController;
    private GameObject characterBody;
    
    public override void Awake() {
        base.Awake();
        base.setup(100, 3.0f);
        this.xRotation = 0.0f;
        this.inventory = new Inventory();
        this.inventory.Add(held, 0, 0);
        this.followTarget = this.transform.GetChild(2);
        this.moveDelta = Vector3.zero;
        this.characterController = this.GetComponent<CharacterController>();
        this.characterBody = this.transform.GetChild(0).gameObject;
    }

    void Update() {
        this.characterController.SimpleMove(this.moveDelta);
        if (this.moveDelta != Vector3.zero) {
            this.characterBody.transform.rotation = Quaternion.LookRotation(this.moveDelta.normalized);
            this.setAsMoving();
        } else {
            this.setAsIdle();
        }
        this.moveDelta = Vector3.zero;
    }

    public Inventory getInventory() {
        return this.inventory;
    }

    public void setMovementDirection(Vector3 newDirection) {
        Quaternion relevantRotation = Quaternion.Euler(0.0f, this.followTarget.rotation.eulerAngles.y, 0.0f);
        this.moveDelta = (relevantRotation * newDirection).normalized * this.baseSpeed;
    }

    public void changeHeld(int index) {
        // this.held = this.inventory[index];
    }

    public void useHeld() {
        Vector3 source = this.followTarget.position;
        Vector3 direction = (this.followTarget.rotation * Vector3.forward).normalized;
        this.held.primaryUsed(this as Shooter, source, direction);
    }

    public void pickupAmmo(int amount) {
        // ((Gun) this.inventory[0]).refillAmmo(amount);
    }

    public void pickupScrap(int amount) {
        // ((Gun) this.inventory[1]).refillAmmo(amount);
    }

    public void changeLookDirection(float mouseDeltaX, float mouseDeltaY) {
        float newClamp = xRotation - mouseDeltaY;
        if (newClamp < 85.0f && newClamp > -85.0f) {
            xRotation -= mouseDeltaY;
            this.followTarget.RotateAround(this.followTarget.position, this.getCameraPivotAngle(), mouseDeltaY);
        }
        Vector3 followTargetRotation = this.followTarget.rotation.eulerAngles;
        followTargetRotation.y += mouseDeltaX;
        followTargetRotation.z = 0;
        this.followTarget.rotation = Quaternion.Euler(followTargetRotation);
    }

    public bool isSelf(GameObject gameObject) {
        return gameObject == this.gameObject;
    }

    public Vector3 getTracerSource() {
        Vector3 source = this.transform.position;
        source.y += 1.0f;
        return source;
    }

    public int getHp() {
        return this.currentHealth;
    }

    public int getCurrentWeaponAmmo() {
        return ((Gun) this.held).getRemainingAmmo();
    }

    public string getCurrentWeaponName() {
        return ((Gun) this.held).getName();
    }

    private Vector3 getCameraPivotAngle() {
        return new Vector3(this.followTarget.forward.z, 0.0f, -this.followTarget.transform.forward.x);
    }
}
