using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeManager : MonoBehaviour
{
    private static DamageOverTimeManager dotm;
    private List<DamageOverTime> dots;
    
    //TODO: WHY NOT BEING CALLED?
    void Awake() {
        if (dotm != null) {
            Debug.LogWarning("Static dotm object already found! Deleting and making another");
            GameObject.Destroy(dotm);
        }
        this.dots = new List<DamageOverTime>();
        dotm = this;
    }

    void Update() {
        List<DamageOverTime> markedForDeletion = new List<DamageOverTime>();
        foreach (DamageOverTime dot in this.dots) {
            if (dot.stillValid()) {
                dot.tick();
            } else {
                markedForDeletion.Add(dot);
            }
        }
        foreach (DamageOverTime dot in markedForDeletion) {
            this.dots.Remove(dot);
        }
    }

    public static DamageOverTime createNewDoT(Mover mover, float dps, float stoppingPower) {
        DamageOverTime newDoT = new DamageOverTime(mover, dps, stoppingPower);
        dotm.dots.Add(newDoT);
        return newDoT;
    }

    public static void removeDoT(DamageOverTime dot) {
        dotm.dots.Remove(dot);
    }
}
