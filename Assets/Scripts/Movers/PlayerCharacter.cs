using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover
{
    public Camera playerCamera;
    public Light playerCamerLight;
    private List<Holdable> inventory;
    private Holdable held;

    public PlayerCharacter() : base(1, 4.5f) {}
    
    void Start() {
        Holdable newRifle = new Rifle();
        Holdable newBuilder = new Builder();
        this.inventory = new List<Holdable>();
        this.inventory.Add(newRifle);
        this.inventory.Add(newBuilder);
        this.held = newRifle;
        this.updateHud();
    }

    void Update() {
        bool anythingChanged = true;
        this.turnCharacterToMouse();
        this.moveCharacter();
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            this.held = this.inventory[0];
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            this.held = this.inventory[1];
        } else if (Input.GetMouseButton(0)) {
            Vector3 source = transform.position;
            Vector3 destination = getMouseLocationAtZeroHeight();
            destination.y = 0.5f;
            this.held.primaryUsed(source, destination);
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

    private void turnCharacterToMouse() {
        Vector3 lookLocation = getMouseLocationAtZeroHeight();
        Vector3 currentLocation = transform.position;
        currentLocation.y = 0;
        Vector3 lookDirection = currentLocation - lookLocation;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void moveCharacter() {
        float horizontalMagnitude = 0.0f;
        float verticalMagnitude = 0.0f;
        horizontalMagnitude += (Input.GetKey("d") ? 1.0f : 0.0f);
        horizontalMagnitude += (Input.GetKey("a") ? -1.0f : 0.0f);
        verticalMagnitude += (Input.GetKey("w") ? 1.0f : 0.0f);
        verticalMagnitude += (Input.GetKey("s") ? -1.0f : 0.0f);
        if (horizontalMagnitude != 0 && verticalMagnitude != 0) {
            horizontalMagnitude *= 0.70710678118f;
            verticalMagnitude *= 0.70710678118f;
        }
        if (horizontalMagnitude != 0 || verticalMagnitude != 0) {
            Vector3 direction = new Vector3(horizontalMagnitude, 0.0f, verticalMagnitude);
            float distance = Time.deltaTime * baseSpeed;
            Vector3 newPos = BoardManager.move(0.5f, transform.position, direction, distance);
            float newX = newPos.x;
            float newZ = newPos.z;
            Vector3 newPlayerLocation = new Vector3(newX, 0.5f, newZ);
            Vector3 newLightLocation = new Vector3(newX, 5.0f, newZ);
            Vector3 newCameraLocation = new Vector3(newX, 18.0f, newZ);
            transform.position = newPlayerLocation;
            this.playerCamerLight.transform.position = newLightLocation;
            this.playerCamera.transform.position = newCameraLocation;
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
        
        HudManager.updateCurrentHeld(this.held.getName());
        HudManager.updateAmmo(ammoAmount);
    }
}
