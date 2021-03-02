using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOTransform", menuName = "ScriptableObjects/SOTransform")]
public class SOTransform : ScriptableObject {
    public Vector3 position;
    public Vector3 rotationEuler;
    public Vector3 scale;

    public void assignTransformation(GameObject gameObject) {
        gameObject.transform.localPosition = this.position;
        this.transformationHelper(gameObject);
    }

    public void applyTransformation(GameObject gameObject) {
        Vector3 newPosition = gameObject.transform.localPosition;
        newPosition.x += this.position.x;
        newPosition.y += this.position.y;
        newPosition.z += this.position.z;
        gameObject.transform.localPosition = newPosition;
        this.transformationHelper(gameObject);
    }

    private void transformationHelper(GameObject gameObject) {
        gameObject.transform.localRotation = Quaternion.Euler(this.rotationEuler);
        gameObject.transform.localScale = this.scale;
    }
}
