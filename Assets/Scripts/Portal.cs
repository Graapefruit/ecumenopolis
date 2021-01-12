﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject enemy;
    private float timeToSpawn = 2.0f;
    
    void Update()
    {
        timeToSpawn -= Time.deltaTime;
        if (timeToSpawn <= 0.0f) {
            Vector3 spawnLocation = transform.position;
            spawnLocation.y = 0.5f;
            GameObject newEnemy = (GameObject) Instantiate(enemy, spawnLocation, Quaternion.identity);
            EnemyManager.addNewEnemy(newEnemy);
            EnemyManager.removeEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
