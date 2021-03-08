using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover, Shooter
{
    private const int PICKUP_LAYER = (1 << 11);
    private const float PICKUP_RANGE = 2.5f;
    public Item startingItem;
    private PlayerInventory inventory;
    private float upRotation;
    private Transform followTarget;
    private Vector3 moveDelta;
    private CharacterController characterController;
    private GameObject characterBody;
    private PlayerCharacterModelHelper modelHelper;
    
    public override void Awake() {
        base.Awake();
        base.setup(100, 3.0f);
        this.upRotation = 0.0f;
        this.followTarget = this.transform.GetChild(2);
        this.moveDelta = Vector3.zero;
        this.characterController = this.GetComponent<CharacterController>();
        this.characterBody = this.transform.GetChild(0).gameObject;
    }

    public void finishInitialization() {
        this.inventory = new PlayerInventory();
        this.modelHelper = new PlayerCharacterModelHelper(this.characterBody, this.animator);
        this.inventory.add(startingItem, 0, 0);
        this.inventory.assignMapping(0, 0, 0);
        this.changeHeld(0);
    }

    void Update() {
        manageHeldItem();
    }

    // Model updates must be called here, to override changes from the animations themselves
    void LateUpdate() {
        manageHorizontalMovement();
        manageVerticalMovement();
    }

    private void manageHorizontalMovement() {
        this.characterController.Move(this.moveDelta * Time.deltaTime);
        this.modelHelper.movementDirection = this.moveDelta;
        this.modelHelper.doUpdate();
    }

    private void manageVerticalMovement() {
        manageHeadBumping();
        // TODO: isGrounded seems to tick on and off. Investigate further. This is why Jumping is GetKey rather than GetKeyDown
        // This must be fixed when falling animations are introduced
        if (this.characterController.isGrounded) {
            this.moveDelta = Vector3.zero;
        } else {
            this.moveDelta.y -= 19.62f * Time.deltaTime;
        }
    }

    private void manageHeadBumping() {
        if ((this.characterController.collisionFlags & CollisionFlags.Above) != 0) {
            this.moveDelta.y = (this.moveDelta.y > 0.0f ? 0.0f : this.moveDelta.y);
        }
    }

    private void manageHeldItem() {
        this.modelHelper.holdItem(this.inventory.getHeld());
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

    public PlayerInventory getInventory() {
        return this.inventory;
    }

    public InventoryHudPanel getInventoryHud() {
        return this.inventory.getHud();
    }

    public HotbarHudPanel getHotbarHud() {
        return this.inventory.getHotbarHud();
    }

    public void setMovement(Vector3 newDirection) {
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
        this.inventory.switchHeld(index);
    }

    public void useHeld() {
        Item held = this.inventory.getHeld();
        if (held != null) {
            Vector3 source = this.followTarget.position;
            Vector3 direction = (this.followTarget.rotation * Vector3.forward).normalized;
            this.inventory.getHeld().primaryUsed(this as Shooter, source, direction);
        }
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
        this.modelHelper.heading += mouseDeltaX;
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

    private Vector3 getCameraPivotAngle() {
        return new Vector3(this.followTarget.forward.z, 0.0f, -this.followTarget.transform.forward.x);
    }
}
