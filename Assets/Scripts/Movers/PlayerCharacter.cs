using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover
{
    public Camera playerCamera;
    public Light playerCamerLight;
    private List<Holdable> inventory;
    private Holdable held;
    private float xRotationClamp;
    
    public override void Awake() {
        base.Awake();
        base.setup(50, 6.25f);
        this.xRotationClamp = 0.0f;
        Holdable newRifle = new Rifle();
        Holdable newBuilder = new Builder();
        this.inventory = new List<Holdable>();
        this.inventory.Add(newRifle);
        this.inventory.Add(newBuilder);
        this.held = newRifle;
    }

    public void changeHeld(int index) {
        this.held = this.inventory[index];
    }

    public void useHeld() {
        Vector3 source = transform.position;
        Vector3 direction = (transform.rotation * Vector3.forward).normalized;
        this.held.primaryUsed(source, direction);
    }

    public void pickupAmmo(int amount) {
        ((Weapon) this.inventory[0]).refillAmmo(amount);
    }

    public void pickupScrap(int amount) {
        ((Weapon) this.inventory[1]).refillAmmo(amount);
    }

    public void changeLookDirection(float mouseDeltaX, float mouseDeltaY) {
        Vector3 playerRotation = transform.rotation.eulerAngles;
        float newClamp = xRotationClamp - mouseDeltaY;
        if (newClamp < 90.0f && newClamp > -90.0f) {
            playerRotation.x -= mouseDeltaY;
            xRotationClamp -= mouseDeltaY;
        }
        playerRotation.y += mouseDeltaX;
        playerRotation.z = 0;
        transform.rotation = Quaternion.Euler(playerRotation);
    }
}
