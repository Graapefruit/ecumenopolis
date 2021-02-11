using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject portal;
    private static EnemyManager em;
    private const float portalBaseCooldown = 2.5f;
    private float remainingPortalCooldown = 2.5f;
    private List<GameObject> enemies;
    
    void Awake() {
        if (em != null) {
            Debug.LogWarning("Static em object already found! Deleting and making another");
            GameObject.Destroy(em);
        }
        em = this;
    }

    void Start() {
        enemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        remainingPortalCooldown -= Time.deltaTime;
        if (remainingPortalCooldown <= 0.0f && !(GameTimeManager.isCurrentlyDay())) {
            Vector3 spawnLocation = BoardManager.getRandomLocation();
            spawnLocation.y = 2.0f;
            enemies.Add((GameObject) Instantiate(portal, spawnLocation, Quaternion.identity));
            remainingPortalCooldown = portalBaseCooldown;
        }
    }

    public static void addNewEnemy(GameObject enemy) {
        em.enemies.Add(enemy);
    }

    public static void removeEnemy(GameObject enemy) {
        em.enemies.Remove(enemy);
    }

    public static void removeAllEnemies() {
        while (em.enemies.Count > 0) {
            GameObject toPurge = em.enemies[0];
            removeEnemy(toPurge);
            Destroy(toPurge);
        }
    }
}
