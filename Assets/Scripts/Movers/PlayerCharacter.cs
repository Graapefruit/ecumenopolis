using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover, Shooter
{
    private const int PICKUP_LAYER = (1 << 11);
    private const float PICKUP_RANGE = 2.5f;
    public Item held;
    private PlayerInventory inventory;
    private float upRotation;
    private float horizontalRotation;
    private Transform followTarget;
    private Vector3 moveDelta;
    private CharacterController characterController;
    private GameObject characterBody;
    private PlayerCharacterModelHelper modelHelper;
    private Vector3 lastDirection;
    
    public override void Awake() {
        base.Awake();
        base.setup(100, 3.0f);
        this.upRotation = 0.0f;
        this.horizontalRotation = 0.0f;
        this.followTarget = this.transform.GetChild(2);
        this.moveDelta = Vector3.zero;
        this.characterController = this.GetComponent<CharacterController>();
        this.characterBody = this.transform.GetChild(0).gameObject;
        this.lastDirection = Vector3.zero;
    }

    public void finishInitialization() {
        this.inventory = new PlayerInventory();
        this.inventory.add(held, 0, 0);
        this.modelHelper = new PlayerCharacterModelHelper(this.characterBody, this.animator);
        this.modelHelper.holdItem(this.held);
    }

    void LateUpdate() {
        this.characterController.Move(this.moveDelta * Time.deltaTime);
        if (this.moveDelta.x != 0.0f || this.moveDelta.z != 0.0f) {
            this.lastDirection = this.moveDelta;
            this.modelHelper.rotateLowerBody(this.lastDirection, this.followTarget.rotation.eulerAngles.y);
        } else {
            this.animator.SetInteger("walkDirection", 0);
        }

        if ((this.characterController.collisionFlags & CollisionFlags.Above) != 0) {
            this.moveDelta.y = 0;
        }

        // TODO: isGrounded seems to tick on and off. Investigate further. This is why Jumping is GetKey rather than GetKeyDown
        // This must be fixed when falling animations are introduced
        if (this.characterController.isGrounded) {
            this.moveDelta = Vector3.zero;
        }
        this.moveDelta.y -= 19.62f * Time.deltaTime;
        this.modelHelper.rotateUpperBody(horizontalRotation);
    }

    public Pickup getFirstPickupInRange() {
        RaycastHit hit;
        Vector3 direction = (this.followTarget.rotation * Vector3.forward).normalized;
        if (Physics.SphereCast(this.followTarget.transform.position, 0.8f, direction, out hit, PICKUP_RANGE, PICKUP_LAYER)) {
            return hit.transform.GetComponent<Pickup>();
        }
        return null;
    }

    public bool addItemIfRoom(Item item) {
        Pair openSlot = this.inventory.getNextOpenSlot();
        if (openSlot == null) {
            return false;
        } else {
            this.inventory.add(item, (int) openSlot.x, (int) openSlot.y);
            return true;
        }
    }

    // public void addItem(Item item, int x, int y) {}

    public Inventory getInventory() {
        return this.inventory;
    }

    public InventoryHudPanel getInventoryHud() {
        return this.inventory.getHud();
    }

    public HotbarHudPanel getHotbarHud() {
        return this.inventory.getHotbarHud();
    }

    public void setMovementDirection(Vector3 newDirection) {
        if (this.characterController.isGrounded) {
            Quaternion relevantRotation = Quaternion.Euler(0.0f, this.followTarget.rotation.eulerAngles.y, 0.0f);
            this.moveDelta = (relevantRotation * newDirection).normalized * this.baseSpeed;
        }
    }

    public void jump() {
        if (this.characterController.isGrounded) {
            moveDelta.y = 8.0f;
        }
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
        float newClamp = upRotation - mouseDeltaY;
        if (newClamp < 85.0f && newClamp > -85.0f) {
            upRotation -= mouseDeltaY;
            this.followTarget.RotateAround(this.followTarget.position, this.getCameraPivotAngle(), mouseDeltaY);
        }
        Vector3 followTargetRotation = this.followTarget.rotation.eulerAngles;
        followTargetRotation.y += mouseDeltaX;
        followTargetRotation.z = 0;
        this.followTarget.rotation = Quaternion.Euler(followTargetRotation);
        this.horizontalRotation += mouseDeltaX;
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
