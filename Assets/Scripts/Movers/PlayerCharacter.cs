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

    public override void dealDamage(int damageDealt, float stoppingPower) {
        base.dealDamage(damageDealt, stoppingPower);
        this.updateHud();
    }
    
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

    public void Start() {
        this.updateHud();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked);
        }
        if (Cursor.lockState == CursorLockMode.Locked) {
            this.changePlayerRotation();
        }
        this.moveCharacter();
        bool anythingChanged = true;
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            this.held = this.inventory[0];
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            this.held = this.inventory[1];
        } else if (Input.GetMouseButton(0)) {
            Vector3 source = transform.position;
            Vector3 direction = (transform.rotation * Vector3.forward).normalized;
            this.held.primaryUsed(source, direction);
        } else {
            anythingChanged = false;
        }

        if (anythingChanged) {
            this.updateHud();
        }
    }

    public void pickupAmmo(int amount) {
        ((Weapon) this.inventory[0]).refillAmmo(amount);
        this.updateHud();
    }

    public void pickupScrap(int amount) {
        ((Weapon) this.inventory[1]).refillAmmo(amount);
        this.updateHud();
    }

    private void changePlayerRotation() {
        float mouseDeltaX = Input.GetAxis("Mouse X");
        float mouseDeltaY = Input.GetAxis("Mouse Y");
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

    private void moveCharacter() {
        float xMagnitude = 0.0f;
        float zMagnitude = 0.0f;
        xMagnitude += (Input.GetKey("d") ? 1.0f : 0.0f);
        xMagnitude += (Input.GetKey("a") ? -1.0f : 0.0f);
        zMagnitude += (Input.GetKey("w") ? 1.0f : 0.0f);
        zMagnitude += (Input.GetKey("s") ? -1.0f : 0.0f);
        Vector3 movementDirection = new Vector3(xMagnitude, 0.0f, zMagnitude).normalized;
        // Apply the current heading to the movement
        movementDirection = transform.rotation * movementDirection;
        // Ignore any elevation in the current heading
        movementDirection.y = 0;
        movementDirection = movementDirection.normalized;
        if (xMagnitude != 0.0f || zMagnitude != 0.0f) {
            this.moveInDirection(movementDirection);
        }
    }

    private Vector3 getMouseLocationAtZeroHeight() {
        Ray ray = this.playerCamera.ScreenPointToRay(Input.mousePosition);
        float distance = -1 * ray.origin.y / ray.direction.y;
        return ray.GetPoint(distance);
    }

    private void updateHud() {
        string weaponName = this.held.getName();
        int ammoAmount = ((Weapon) this.held).getRemainingAmmo();
        
        HudManager.updateHealth(this.currentHealth);
        HudManager.updateCurrentHeld(this.held.getName());
        HudManager.updateAmmo(ammoAmount);
    }
}
