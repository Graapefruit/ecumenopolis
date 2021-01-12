using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    void OnTriggerEnter(Collider collider) {
        GameObject gameObject = collider.gameObject;
        if (this.validPickupCandidate(gameObject)) {
            this.getPickedUpBy(gameObject);
        }
    }

    protected abstract bool validPickupCandidate(GameObject gameObject);
    protected abstract void getPickedUpBy(GameObject gameObject);
    protected bool gameObjectIsOfClass<T>(GameObject gameObject) {
        return gameObject.GetComponents(typeof(T)).Length > 0;
    }
    protected T getClassFromGameObject<T>(GameObject gameObject) where T : class {
        return gameObject.GetComponents(typeof(T))[0] as T;
    }
}
