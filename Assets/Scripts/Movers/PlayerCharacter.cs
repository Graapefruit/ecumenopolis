using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover
{
    private readonly float baseSpeed = 4.5f;
    private readonly int baseHealth = 1;
    public Camera camera;
    public Light light;
    private List<Holdable> inventory;
    private Holdable held;
    private int scrapCount;
    
    void Start()
    {
        Holdable newRifle = new Rifle();
        Holdable newBuilder = new Builder();
        this.inventory = new List<Holdable>();
        this.inventory.Add(newRifle);
        this.inventory.Add(newBuilder);
        this.scrapCount = 0;
        this.held = newRifle;
        this.updateHud();
    }

    // Update is called once per frame
    void Update()
    {
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
        this.scrapCount = amount;
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
        float horizontalMagnitude = 0;
        float verticalMagnitude = 0;
        horizontalMagnitude += (Input.GetKey("right") ? 1.0f : 0.0f);
        horizontalMagnitude += (Input.GetKey("left") ? -1.0f : 0.0f);
        verticalMagnitude += (Input.GetKey("up") ? 1.0f : 0.0f);
        verticalMagnitude += (Input.GetKey("down") ? -1.0f : 0.0f);
        if (horizontalMagnitude !=0 && verticalMagnitude != 0) {
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
            light.transform.position = newLightLocation;
            camera.transform.position = newCameraLocation;
        }
    }

    private Vector3 getMouseLocationAtZeroHeight() {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        // The below line is derrived from: origin.y + n*direction.y = 0
        float distance = -1 * ray.origin.y / ray.direction.y;
        return ray.GetPoint(distance);
    }

    private void updateHud() {
        int remainingAmmo = 0;
        HudManager.updateCurrentHeld(this.held.getName());
        if (this.held is Weapon) {
            remainingAmmo = ((Weapon) this.held).getRemainingAmmo();
        }
        HudManager.updateAmmo(remainingAmmo);
        HudManager.updateScrap(this.scrapCount);
    }
}
