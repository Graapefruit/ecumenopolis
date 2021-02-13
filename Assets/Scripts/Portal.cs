using System.Collections;
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
            GameObject newEnemy = (GameObject) Instantiate(enemy, transform.position, Quaternion.identity);
            newEnemy.GetComponent<Enemy>().giveTarget(Game.getPlayer());
            EnemyManager.addNewEnemy(newEnemy);
            EnemyManager.removeEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
