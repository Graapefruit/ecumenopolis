using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterModelHelper {
    private GameObject playerModel;
    private GameObject rightHand;
    private Item heldItem;
    private GameObject heldGameObject;

    public PlayerCharacterModelHelper(GameObject playerModel) {
        this.playerModel = playerModel;
        this.rightHand = playerModel.transform
            .Find("Armature")
            .Find("Hips")
            .Find("Spine")
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
}