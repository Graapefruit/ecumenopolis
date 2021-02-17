using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover, Shooter
{
    private List<Holdable> inventory;
    private Holdable held;
    private float xRotation;
    private Transform thirdPersonCamera;
    private Vector3 moveDelta;
    private CharacterController characterController;
    
    public override void Awake() {
        base.Awake();
        base.setup(100, 3.0f);
        this.xRotation = 0.0f;
        Holdable newRifle = new Rifle();
        Holdable newBuilder = new Builder();
        this.inventory = new List<Holdable>();
        this.inventory.Add(newRifle);
        this.inventory.Add(newBuilder);
        this.held = newRifle;
        this.thirdPersonCamera = this.transform.GetChild(1);
        this.moveDelta = Vector3.zero;
        this.characterController = this.GetComponent<CharacterController>();
    }

    void Update() {
        this.characterController.SimpleMove(this.moveDelta);
        if (this.moveDelta != Vector3.zero) {
            this.setAsMoving();
        } else {
            this.setAsIdle();
        }
        this.moveDelta = Vector3.zero;
    }

    public void setMovementDirection(Vector3 newDirection) {
        this.moveDelta = newDirection * this.baseSpeed;
    }

    public void changeHeld(int index) {
        this.held = this.inventory[index];
    }

    public void useHeld() {
        Vector3 source = this.thirdPersonCamera.position;
        Vector3 direction = (this.thirdPersonCamera.rotation * Vector3.forward).normalized;
        this.held.primaryUsed(this as Shooter, source, direction);
    }

    public void pickupAmmo(int amount) {
        ((Weapon) this.inventory[0]).refillAmmo(amount);
    }

    public void pickupScrap(int amount) {
        ((Weapon) this.inventory[1]).refillAmmo(amount);
    }

    public void changeLookDirection(float mouseDeltaX, float mouseDeltaY) {
        Vector3 playerRotation = this.transform.rotation.eulerAngles;
        float newClamp = xRotation - mouseDeltaY;
        if (newClamp < 90.0f && newClamp > -90.0f) {
            xRotation -= mouseDeltaY;
            this.thirdPersonCamera.RotateAround(this.transform.position, this.getCameraPivotAngle(), mouseDeltaY);
            
        }
        playerRotation.y += mouseDeltaX;
        playerRotation.z = 0;
        transform.rotation = Quaternion.Euler(playerRotation);
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
        return ((Weapon) this.held).getRemainingAmmo();
    }

    public string getCurrentWeaponName() {
        return ((Weapon) this.held).getName();
    }

    private Vector3 getCameraPivotPoint() {
        Vector3 pivotPoint = this.transform.position + this.thirdPersonCamera.position;
        pivotPoint.z += 1.0f;
        return pivotPoint;
    }

    private Vector3 getCameraPivotAngle() {
        return new Vector3(transform.forward.z, 0.0f, -transform.forward.x);
    }
}
