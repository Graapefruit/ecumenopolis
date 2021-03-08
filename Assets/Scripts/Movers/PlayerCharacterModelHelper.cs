using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterModelHelper {
    private const float ANIMATION_LAYER_ON = 1.0f;
    private const float ANIMATION_LAYER_OFF = 0.0f;
    private const float LOWER_BODY_INITIAL_ROTATION = -90.0f; 
    public float heading;
    public Vector3 movementDirection;
    private float lowerBodyRotation;
    private GameObject playerModel;
    private Animator animator;
    private GameObject lowerBodyStart;
    private GameObject upperBodyStart;
    private GameObject rightHand;
    private Item heldItem;
    private GameObject heldGameObject;
    private PlayerSwivelHelper swivelHelper;

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
    }

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

    public void doUpdate() {
        this.rotateLowerBody();
        this.rotateUpperBody();
    }

    private void rotateLowerBody() {
        if (this.movementDirection.x == 0.0f && this.movementDirection.z == 0.0f) {
            this.animator.SetInteger("walkDirection", 0);
            this.swivelHelper.manageSwivel(this.heading, this.lowerBodyRotation);
            Debug.Log(this.lowerBodyRotation);
            this.lowerBodyRotation += Time.deltaTime * this.swivelHelper.getSwivelAmountWithoutDeltaTime();
        } else {
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
        }
        Vector3 lowerBodyEuler = this.lowerBodyStart.transform.eulerAngles;
        lowerBodyEuler.y += this.lowerBodyRotation;
        this.lowerBodyStart.transform.rotation = Quaternion.Euler(lowerBodyEuler);
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

    private void rotateUpperBody() {
        Vector3 upperBodyEuler = this.upperBodyStart.transform.eulerAngles;
        upperBodyEuler.y = this.heading;
        this.upperBodyStart.transform.rotation = Quaternion.Euler(upperBodyEuler);
    }

    private void setLayerWeight(string layerName, float newWeight) {
        int relevantLayerIndex = this.animator.GetLayerIndex(layerName);
        this.animator.SetLayerWeight(relevantLayerIndex, newWeight);
    }
}