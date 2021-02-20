using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeldItemTransform", menuName = "ScriptableObjects/HeldItemTransform")]
public class PlayerCharacterHoldTransform : ScriptableObject {
    public Vector3 relativePos;
    public Vector3 relativeRotEuler;
    public Vector3 relativeScale;

    public void assignTransformation(GameObject gameObject) {
        gameObject.transform.localPosition = this.relativePos;
        gameObject.transform.localRotation = Quaternion.Euler(this.relativeRotEuler);
        gameObject.transform.localScale = this.relativeScale;
    }
}
