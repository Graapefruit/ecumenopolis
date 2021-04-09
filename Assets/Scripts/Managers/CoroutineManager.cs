using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour {
    private static CoroutineManager ch;
    
    void Awake() {
        if (ch != null) {
            Debug.LogWarning("Static ch object already found! Deleting and making another");
            GameObject.Destroy(ch);
        }
        ch = this;
    }

    public static void doCoroutine(IEnumerator coroutine) {
        ch.doCoroutineHelper(coroutine);
    }

    public static void stopCoroutine(IEnumerator coroutine) {
        ch.stopCoroutineHelper(coroutine);
    }

    private void doCoroutineHelper(IEnumerator coroutine) {
        StartCoroutine(coroutine);
    }

    private void stopCoroutineHelper(IEnumerator coroutine) {
        StopCoroutine(coroutine);
    }
}