using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    protected readonly float baseSpeed;
    protected readonly int baseHealth;
    protected int currentHealth;
    // Start is called before the first frame update
    void Start() {
        currentHealth = baseHealth;
    }

    public void dealDamage(int damageDealt) {
        this.currentHealth -= damageDealt;
        if (currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    protected void moveInDirection(Vector3 directionVector) {
        float newX = transform.position.x + (directionVector.x * Time.deltaTime);
        float newY = transform.position.y;
        float newZ = transform.position.z + (directionVector.z * Time.deltaTime);
        transform.position = new Vector3(newX, newY, newZ);
    }

    protected void moveTowardsLocation(Vector3 locationVector) {
        transform.position = Vector3.MoveTowards(transform.position, locationVector, baseSpeed * Time.deltaTime);
    }
}
