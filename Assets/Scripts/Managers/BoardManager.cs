using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoardManager : MonoBehaviour {
    public static Vector3 getRandomLocation() {
        int iterations = 0;
        NavMeshHit navHit;
        while (iterations < 16) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 50.0f;
            randomDirection += new Vector3(28.0f, 7.0f, 14.0f);
            NavMesh.SamplePosition (randomDirection, out navHit, 5.0f, NavMesh.AllAreas);
            if (navHit.hit) {
                return navHit.position;
            }
            iterations++;
        }
        return Vector3.zero;
    }

    public static Vector3 getClosestDropLocation(Vector3 source) {
        NavMeshHit navHit;
        NavMesh.SamplePosition (source, out navHit, 5.0f, NavMesh.AllAreas);
        return navHit.position;
    }
}
