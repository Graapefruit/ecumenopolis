using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover, Shooter {
    public Item startingItem;
    public GameObject inventoryHudPrefab;
    public GameObject inventoryHotbarHudPrefab;
    public const float MAX_STAMINA = 5.0f;
    public const float MIN_STAMINA = 0.0f;
    private const int PICKUP_LAYER = (1 << 11);
    private const float PICKUP_RANGE = 2.5f;
    private const float WALK_SPEED = 4.2f;
    private const float SPRINT_SPEED = 6.0f;
    private const float STAMINA_RECOVERY_RATE_IDLE = 0.85f;
    private const float STAMINA_RECOVERY_RATE_WALKING = 0.65f;
    private const float STAMINA_DRAIN_RATE_SPRINTING = -1.5f;
    private const float JUMP_STAMINA_COST = 1.5f;
    public bool sprinting;
    public bool shooting;
    public Vector3 HorizontalDirection {
        get { return this.horizontalDirection; }
        set { 
            if (this.characterController.isGrounded) {
                Quaternion relevantRotation = Quaternion.Euler(0.0f, this.followTarget.rotation.eulerAngles.y, 0.0f);
                this.horizontalDirection = (relevantRotation * value).normalized;
            }
        }
    }
    private Vector3 horizontalDirection;
    private float verticalDirection;
    private float currentStamina;
    private float upRotation;
    private PlayerInventory inventory;
    private Transform followTarget;
    private CharacterController characterController;
    private PlayerCharacterModelHelper modelHelper;
    private StateManager stateManager;
    
    public override void Awake() {
        base.Awake();
        base.setup(100, 3.0f);
        this.sprinting = false;
        this.shooting = false;
        this.horizontalDirection = Vector3.zero;
        this.verticalDirection = 0.0f;
        this.currentStamina = MAX_STAMINA;
        this.upRotation = 0.0f;
        this.followTarget = this.transform.GetChild(2);
        this.characterController = this.GetComponent<CharacterController>();
        this.initializeStates();
        InventoryHudPanel hud = Instantiate(inventoryHudPrefab, Vector3.zero, Quaternion.identity).GetComponent<InventoryHudPanel>();
        HotbarHudPanel hotbarHud = Instantiate(inventoryHotbarHudPrefab, Vector3.zero, Quaternion.identity).GetComponent<HotbarHudPanel>();
        this.inventory = new PlayerInventory(hud, hotbarHud);
        this.inventory.add(startingItem, 0, 0);
        this.inventory.assignMapping(0, 0, 0);
        this.modelHelper = new PlayerCharacterModelHelper(this.transform.GetChild(0).gameObject, this.animator);
    }

    void Start() {
    }

    // Model updates must be called in LATE update to override changes from the animations themselves
    void LateUpdate() {
        this.stateManager.doUpdate();
        this.modelHelper.doUpdate();
        // Debug.Log(this.stateManager.getCurrentStateName());
    }

    private void initializeStates() {
        State idleState = new State(
            "idle",
            (() => {
                this.currentSpeed = 0.0f;
                this.modelHelper.movementDirection = Vector3.zero;
            }),
            (() => {
                modifyStamina(STAMINA_RECOVERY_RATE_IDLE);
                manageHeldItem();
                manageShooting();
            }),
            (() => {})
        );

        State walkingState = new State(
            "walking",
            (() => {
                this.currentSpeed = WALK_SPEED;
            }),
            (() => { 
                modifyStamina(STAMINA_RECOVERY_RATE_WALKING);
                manageHeldItem();
                manageHorizontalMovement();
                manageShooting();
            }),
            (() => {})
        );

        State sprintingState = new State(
            "sprinting",
            (() => {
                this.currentSpeed = SPRINT_SPEED;
                this.modelHelper.shooting = false;
            }),
            (() => {
                modifyStamina(STAMINA_DRAIN_RATE_SPRINTING);
                manageHeldItem();
                manageHorizontalMovement();
            }),
            (() => {})
        );

        State midairState = new State(
            "midair",
            (() => {
                
            }),
            (() => {
                manageHeldItem();
                manageHorizontalMovement();
                manageVerticalMovement();
                manageShooting();
            }),
            (() => {})
        );

        // TODO: Jumping works badly rn. Relies on the inconsistencies of isGrounded to begin jumping
        idleState.setOnGetNextState(() => {
            if (!this.characterController.isGrounded || this.verticalDirection != 0.0f) {
                return midairState;
            } else if (this.horizontalDirection != Vector3.zero) {
                return walkingState;
            } else {
                return idleState;
            }
        });

        walkingState.setOnGetNextState(() => {
            if (!this.characterController.isGrounded || this.verticalDirection != 0.0f) {
                return midairState;
            } else if (this.sprinting) {
                return sprintingState;
            } else if (this.horizontalDirection == Vector3.zero) {
                return idleState;
            } else {
                return walkingState;
            }
        });

        sprintingState.setOnGetNextState(() => {
            if (!this.characterController.isGrounded || this.verticalDirection != 0.0f) {
                return midairState;
            } else if (this.horizontalDirection == Vector3.zero) {
                return idleState;
            } else if (!this.sprinting) {
                return walkingState;
            } else {
                return sprintingState;
            }
        });

        midairState.setOnGetNextState(() => {
            if (this.characterController.isGrounded) {
                if (this.horizontalDirection == Vector3.zero) {
                    return idleState;
                } else if (this.sprinting) {
                    return sprintingState;
                } else {
                    return walkingState;
                }
            } else {
                return midairState;
            }
        });

        this.stateManager = new StateManager(idleState);
    }

    private void manageHorizontalMovement() {
        this.characterController.Move(this.horizontalDirection * this.currentSpeed * Time.deltaTime);
        this.modelHelper.movementDirection = this.horizontalDirection * this.currentSpeed;
    }

    private void manageVerticalMovement() {
        // TODO: isGrounded seems to tick on and off. Investigate further. This is why Jumping is GetKey rather than GetKeyDown
        // This must be fixed when falling animations are introduced
        this.characterController.Move(new Vector3(0.0f, this.verticalDirection * Time.deltaTime, 0.0f));
        if (this.characterController.isGrounded) {
            this.verticalDirection = 0.0f;
        } else {
            manageHeadBumping();
            this.verticalDirection -= 19.62f * Time.deltaTime;
        }
    }

    private void manageHeadBumping() {
        if ((this.characterController.collisionFlags & CollisionFlags.Above) != 0) {
            this.verticalDirection = (this.verticalDirection > 0.0f ? 0.0f : this.verticalDirection);
        }
    }

    private void manageHeldItem() {
        this.modelHelper.holdItem(this.inventory.getHeld());
    }

    private void manageShooting() {
        if (this.shooting) {
            Item held = this.inventory.getHeld();
            if (held != null) {
                Vector3 source = this.followTarget.position;
                Vector3 direction = (this.followTarget.rotation * Vector3.forward).normalized;
                this.inventory.getHeld().primaryUsed(this as Shooter, source, direction);
            }
        }
        this.modelHelper.shooting = this.shooting;
    }

    private void modifyStamina(float amount) {
        float newStamina = this.currentStamina + (Time.deltaTime * amount);
        this.currentStamina = Mathf.Max(MIN_STAMINA, Mathf.Min(MAX_STAMINA, newStamina));
    }

    private void cancelReload() {
        Item held = this.inventory.getHeld();
        if (held != null && held is Gun) {
            Gun heldGun = this.inventory.getHeld() as Gun;
            if (heldGun.isReloading()) {
                this.modelHelper.stopReload();
                heldGun.cancelReload();
            }
        }
    }

    // ================== PUBLIC METHODS ==================

    public void reload() {
        // TODO: this.modelHelper.beginReload();
        Item held = this.inventory.getHeld();
        if (held != null && held is Gun) {
            Gun heldGun = this.inventory.getHeld() as Gun;
            if (!heldGun.isReloading()) {
                this.modelHelper.startReload();
                heldGun.startReload(this.modelHelper.stopReload, this.inventory.getAmmoForHeldAndSpendAmmo);
            }
        }
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

    public PlayerInventory getInventory() {
        return this.inventory;
    }

    public void jump() {
        if (this.characterController.isGrounded) {
            float jumpStrength = Mathf.Min(1.0f, this.currentStamina / JUMP_STAMINA_COST);
            verticalDirection = 8.0f * jumpStrength;
            this.currentStamina = Mathf.Max(this.currentStamina - JUMP_STAMINA_COST, 0.0f);
        }
    }

    public void changeHeld(int index) {
        this.cancelReload();
        this.inventory.switchHeld(index);
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

    private Vector3 getCameraPivotAngle() {
        return new Vector3(this.followTarget.forward.z, 0.0f, -this.followTarget.transform.forward.x);
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

    public float getStamina() {
        return this.currentStamina;
    }
}
