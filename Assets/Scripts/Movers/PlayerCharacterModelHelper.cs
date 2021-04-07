using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterModelHelper {
    private const float ANIMATION_LAYER_ON = 1.0f;
    private const float ANIMATION_LAYER_OFF = 0.0f;
    private const float LOWER_BODY_INITIAL_ROTATION = -90.0f; 
    public float heading;
    public Vector3 movementDirection;
    public bool shooting;
    private float lowerBodyRotation;
    private GameObject playerModel;
    private Animator animator;
    private GameObject lowerBodyStart;
    private GameObject upperBodyStart;
    private GameObject rightHand;
    private Item heldItem;
    private GameObject heldGameObject;
    private PlayerSwivelHelper swivelHelper;
    private StateManager upperBodyStateManager;
    private StateManager lowerBodyStateManager;

    public PlayerCharacterModelHelper(GameObject playerModel, Animator animator) {
        this.heading = 0.0f;
        this.playerModel = playerModel;
        this.animator = animator;
        this.swivelHelper = new PlayerSwivelHelper(this.animator);
        this.lowerBodyStart = playerModel.transform.Find("Armature").Find("Hips").gameObject;
        this.upperBodyStart = this.lowerBodyStart.transform.Find("Spine").gameObject;
        this.rightHand = this.upperBodyStart.transform
            .Find("Spine1")
            .Find("Spine2")
            .Find("RightShoulder")
            .Find("RightArm")
            .Find("RightForeArm")
            .Find("RightHand").gameObject;
            this.initializeStateManager();
    }

    private void initializeStateManager() {
        this.initializeUpperBodyStateManager();
        this.initializeLowerBodyStateManager();
    }

    private void initializeUpperBodyStateManager() {
        State reactiveState = new State(
            "reactive",
            (() => {}),
            (() => {
                setUpperBodyRotation();
            }),
            (() => {})
        );

        State shootingState = new State(
            "shooting",
            (() => {
                this.animator.SetBool("shooting", true);
            }),
            (() => {
                setUpperBodyRotation();
            }),
            (() => {
                this.animator.SetBool("shooting", false);
            })
        );

        reactiveState.setOnGetNextState(() => {
            if (this.shooting) {
                return shootingState;
            } else {
                return reactiveState;
            }
        });

        shootingState.setOnGetNextState(() => {
            if (!this.shooting) {
                return reactiveState;
            } else {
                return shootingState;
            }
        });
        
        this.upperBodyStateManager = new StateManager(reactiveState);
    }

    private void initializeLowerBodyStateManager() {
        State idleState = new State(
            "idle",
            (() => {
                this.animator.SetInteger("walkDirection", 0);
            }),
            (() => {
                this.swivelHelper.manageSwivel(this.heading, this.lowerBodyRotation);
                this.lowerBodyRotation += Time.deltaTime * this.swivelHelper.getSwivelAmountWithoutDeltaTime();
                setLowerBodyRotation();
        }),
            (() => {})
        );

        State walkingState = new State(
            "walking",
            (() => {}),
            (() => {
                this.swivelHelper.stopSwiveling();
                this.snapLowerBodyToMovement();
                float contortionAngle = this.calculateContortion();
                if (contortionAngle > 300 || contortionAngle < 60) {
                    this.animator.SetInteger("walkDirection", 1);
                } else if (contortionAngle >= 240 && contortionAngle <= 300) {
                    this.lowerBodyRotation += 90;
                    this.animator.SetInteger("walkDirection", 2);
                } else if (contortionAngle >= 60 && contortionAngle <= 120) {
                    this.lowerBodyRotation -= 90;
                    this.animator.SetInteger("walkDirection", 3);
                } else {
                    this.lowerBodyRotation += 180;
                    this.animator.SetInteger("walkDirection", 4);
                }
                setLowerBodyRotation();
            }),
            (() => {})
        );

        idleState.setOnGetNextState(() => {
            if (this.movementDirection != Vector3.zero) {
                return walkingState;
            } else {
                return idleState;
            }
        });

        walkingState.setOnGetNextState(() => {
            if (this.movementDirection == Vector3.zero) {
                return idleState;
            } else {
                return walkingState;
            }
        });
        
        this.lowerBodyStateManager = new StateManager(idleState);
    }

    public void doUpdate() {
        // The bone for the lower body is the parent of all bones, so it should be updated first, so it's effects on the lower body can be overwritten
        this.lowerBodyStateManager.doUpdate();
        this.upperBodyStateManager.doUpdate();
        // Debug.Log(this.upperBodyStateManager.getCurrentStateName());
    }

    private void setLowerBodyRotation() {
        Vector3 lowerBodyEuler = this.lowerBodyStart.transform.eulerAngles;
        lowerBodyEuler.y += this.lowerBodyRotation;
        this.lowerBodyStart.transform.rotation = Quaternion.Euler(lowerBodyEuler);
    }

    private void setUpperBodyRotation() {
        Vector3 upperBodyEuler = this.upperBodyStart.transform.eulerAngles;
        upperBodyEuler.y = this.heading;
        this.upperBodyStart.transform.rotation = Quaternion.Euler(upperBodyEuler);
    }

    private void setLayerWeight(string layerName, float newWeight) {
        int relevantLayerIndex = this.animator.GetLayerIndex(layerName);
        this.animator.SetLayerWeight(relevantLayerIndex, newWeight);
    }

    private void snapLowerBodyToMovement() {
        this.lowerBodyRotation = LOWER_BODY_INITIAL_ROTATION + (Mathf.Atan2(this.movementDirection.z, -this.movementDirection.x) * Mathf.Rad2Deg);
    }

    private float calculateContortion() {
        float contortionAngle = this.lowerBodyRotation - this.heading;
        while (contortionAngle < 0) {
            contortionAngle += 360;
        }
        return contortionAngle;
    }

    // ================== PUBLIC METHODS ==================

    public void holdItem(Item newHeldItem) {
        if (this.heldItem == newHeldItem) {
            return;
        }
        if (this.heldItem != null) {
            setLayerWeight(this.heldItem.heldAnimationLayerName, ANIMATION_LAYER_OFF);
            GameObject.Destroy(this.heldGameObject);
        }
        this.heldItem = newHeldItem;
        if (newHeldItem != null) {
            this.heldGameObject = GameObject.Instantiate(newHeldItem.prefab, Vector3.zero, Quaternion.identity) as GameObject;
            this.heldGameObject.transform.SetParent(this.rightHand.transform);
            this.heldItem.holdTransform.assignTransformation(this.heldGameObject);
            setLayerWeight(this.heldItem.heldAnimationLayerName, ANIMATION_LAYER_ON);
        }
    }

    public void startReload() {
        this.animator.SetBool("reloading", true);
    }

    public void stopReload() {
        this.animator.SetBool("reloading", false);
    }
}