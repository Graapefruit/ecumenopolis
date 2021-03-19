using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Mover, Shooter {
    public const float MAX_STAMINA = 5.0f;
    public Item startingItem;
    private const int PICKUP_LAYER = (1 << 11);
    private const float PICKUP_RANGE = 2.5f;
    private const float SPRINT_SPEED = 5.0f;
    private const float SPRINT_COOLDOWN = 1.5f;
    private const float SPRINT_COST = 1.5f;
    private const float STAMINA_RECOVERY_RATE = 0.75f;
    // public bool Sprinting {
    //     get { return sprinting; }
    //     set {
    //         if (value) {
    //             if (Time.time > (this.lastSprintTime + SPRINT_COOLDOWN)) {
    //                 sprinting = true;
    //                 this.currentSpeed = SPRINT_SPEED;
    //             }
    //         } else {
    //             this.lastSprintTime = Time.time;
    //             this.sprinting = false;
    //             this.currentSpeed = this.baseSpeed;
    //         }
    //     }
    // }
    private float lastSprintTime;
    private float currentStamina;
    private float upRotation;
    private PlayerInventory inventory;
    private Transform followTarget;
    private Vector3 moveDelta;
    private CharacterController characterController;
    private PlayerCharacterModelHelper modelHelper;
    private StateManager stateManager;
    
    public override void Awake() {
        base.Awake();
        base.setup(100, 3.0f);
        this.lastSprintTime = -SPRINT_COOLDOWN;
        this.currentStamina = MAX_STAMINA;
        this.upRotation = 0.0f;
        this.followTarget = this.transform.GetChild(2);
        this.moveDelta = Vector3.zero;
        this.characterController = this.GetComponent<CharacterController>();
        this.initializeStates();
    }

    // TODO: Start?
    public void finishInitialization() {
        this.inventory = new PlayerInventory();
        this.modelHelper = new PlayerCharacterModelHelper(this.transform.GetChild(0).gameObject, this.animator);
        this.inventory.add(startingItem, 0, 0);
        this.inventory.assignMapping(0, 0, 0);
    }

    void Update() {
        // manageHeldItem();
        // updateStamina();
    }

    // Model updates must be called in LATE update to override changes from the animations themselves
    void LateUpdate() {
        // manageHorizontalMovement();
        // manageVerticalMovement();
        this.stateManager.doUpdate();
    }

    private void initializeStates() {
        State idleState = new State(
            (() => {}),
            (() => {}),
            (() => {})
        );

        State walkingState = new State(
            (() => {}),
            (() => { 
                manageHorizontalMovement();
            }),
            (() => {})
        );

        State midairState = new State(
            (() => {}),
            (() => {
                manageHorizontalMovement();
                manageVerticalMovement();
            }),
            (() => {})
        );

        idleState.setOnGetNextState(() => {
            if (!this.characterController.isGrounded) {
                return midairState;
            } else if (moveDelta.x != 0.0f || moveDelta.z != 0.0f) {
                return walkingState;
            } else {
                return idleState;
            }
        });

        walkingState.setOnGetNextState(() => {
            if (!this.characterController.isGrounded) {
                return midairState;
            } else if (moveDelta == Vector3.zero) {
                return idleState;
            } else {
                return walkingState;
            }
        });

        midairState.setOnGetNextState(() => {
            if (this.characterController.isGrounded) {
                if (moveDelta == Vector3.zero) {
                    return idleState;
                } else {
                    return walkingState;
                }
            } else {
                return midairState;
            }
        });

        this.stateManager = new StateManager(idleState);
    }

    private void maintainAnimation() {
        
    }

    public void reload() {
        // TODO: this.modelHelper.beginReload();
        this.inventory.reloadHeldWeapon();
    }

    // private void updateStamina() {
    //     if (this.sprinting) {
    //         this.currentStamina -= Time.deltaTime * SPRINT_COST;
    //         if (this.currentStamina < 0.0f) {
    //             this.Sprinting = false;
    //         }
    //     } else {
    //         float newStamina = this.currentStamina + (Time.deltaTime * STAMINA_RECOVERY_RATE);
    //         this.currentStamina = (newStamina < MAX_STAMINA ? newStamina : MAX_STAMINA);
    //     }
    // }

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

    public PlayerInventory getInventory() {
        return this.inventory;
    }

    public void setMovement(Vector3 newDirection) {
        if (this.characterController.isGrounded) {
            Quaternion relevantRotation = Quaternion.Euler(0.0f, this.followTarget.rotation.eulerAngles.y, 0.0f);
            this.moveDelta = (relevantRotation * newDirection).normalized * this.currentSpeed;
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

    public float getStamina() {
        return this.currentStamina;
    }

    private Vector3 getCameraPivotAngle() {
        return new Vector3(this.followTarget.forward.z, 0.0f, -this.followTarget.transform.forward.x);
    }
}
