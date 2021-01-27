using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caltrops : MonoBehaviour {
    private const int baseHealth = 45;
    private const float stoppingPower = 3.5f;
    private const float dps = 2.0f;
    // private int currentHealth;
    // TODO: If enemy dies, needs to be removed from here. 
    private Dictionary<Enemy, DamageOverTime> enemiesCaught;

    public Caltrops() {
        // this.currentHealth = baseHealth;
        enemiesCaught = new Dictionary<Enemy, DamageOverTime>();
    }

    void OnTriggerEnter(Collider collider) {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy) {
            enemiesCaught.Add(enemy, DamageOverTimeManager.createNewDoT(enemy, dps, stoppingPower));
        }
    }

    void OnTriggerExit(Collider collider) {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy) {
            DamageOverTimeManager.removeDoT(enemiesCaught[enemy]);
            enemiesCaught.Remove(enemy);
        }
    }
}
