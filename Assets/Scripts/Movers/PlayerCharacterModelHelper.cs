using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterModelHelper {
    private GameObject playerModel;
    private Animator animator;
    private GameObject lowerBodyStart;
    private float lowerBodyInitialRotation; 
    private GameObject upperBodyStart;
    private GameObject rightHand;
    private Item heldItem;
    private GameObject heldGameObject;

    public PlayerCharacterModelHelper(GameObject playerModel, Animator animator) {
        this.playerModel = playerModel;
        this.animator = animator;
        this.lowerBodyStart = playerModel.transform.Find("Armature").Find("Hips").gameObject;
        this.lowerBodyInitialRotation = -90.0f;
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
        if (heldItem != null) {

        }
        this.heldItem = newHeldItem;
        this.heldGameObject = GameObject.Instantiate(newHeldItem.prefab, Vector3.zero, Quaternion.identity) as GameObject;
        this.heldGameObject.transform.SetParent(this.rightHand.transform);
        this.heldItem.holdTransform.assignTransformation(this.heldGameObject);
    }

    public void rotateLowerBody(Vector3 direction, float headingAngle) {
        Vector3 lowerBodyEuler = this.lowerBodyStart.transform.eulerAngles;
        Vector3 upperBodyEuler = this.upperBodyStart.transform.eulerAngles;
        float angle = this.lowerBodyInitialRotation + (Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg);
        float contortionAngle = angle - headingAngle;
        while (contortionAngle < 0) {
            contortionAngle += 360;
        }
        if (contortionAngle > 300 || contortionAngle < 60) {
            this.animator.SetInteger("walkDirection", 1);
        } else if (contortionAngle >= 240 && contortionAngle <= 300) {
            angle += 90;
            this.animator.SetInteger("walkDirection", 2);
        } else if (contortionAngle >= 60 && contortionAngle <= 120) {
            angle -= 90;
            this.animator.SetInteger("walkDirection", 3);
        } else {
            angle += 180;
            this.animator.SetInteger("walkDirection", 4);
        }
        lowerBodyEuler.y += angle;
        this.lowerBodyStart.transform.rotation = Quaternion.Euler(lowerBodyEuler);
        this.upperBodyStart.transform.rotation = Quaternion.Euler(upperBodyEuler);
    }

    public void rotateUpperBody(float rotationAmount) {
        Vector3 upperBodyEuler = this.upperBodyStart.transform.eulerAngles;
        upperBodyEuler.y += rotationAmount;
        this.upperBodyStart.transform.rotation = Quaternion.Euler(upperBodyEuler);
    }
}