using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public GameObject pcPrefab;
    private PlayerCharacter playerCharacter;

    void Awake() {
        GameObject newPc = (GameObject) Instantiate(pcPrefab, new Vector3 (0.0f, 0.5f, 0.0f), Quaternion.identity);
        this.playerCharacter = newPc.GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        manageCameraLocking();
        if (Cursor.lockState == CursorLockMode.Locked) {
            manageMouseMovements();
            manageMovement();
            manageInventory();
            manageGunShooting();
        }
    }

    public PlayerCharacter getPlayer() {
        return this.playerCharacter;
    } 

    private void manageCameraLocking() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked);
        }
    }

    private void manageMouseMovements() {
        float mouseDeltaX = Input.GetAxis("Mouse X");
        float mouseDeltaY = Input.GetAxis("Mouse Y");
        if (mouseDeltaX != 0 || mouseDeltaY != 0) {
            playerCharacter.changeLookDirection(mouseDeltaX, mouseDeltaY);
        }
    }

    private void manageMovement() {
        float xMagnitude = 0.0f;
        float zMagnitude = 0.0f;
        xMagnitude += (Input.GetKey("d") ? 1.0f : 0.0f);
        xMagnitude += (Input.GetKey("a") ? -1.0f : 0.0f);
        zMagnitude += (Input.GetKey("w") ? 1.0f : 0.0f);
        zMagnitude += (Input.GetKey("s") ? -1.0f : 0.0f);
        Vector3 movementDirection = new Vector3(xMagnitude, 0.0f, zMagnitude).normalized;
        // Apply the current heading to the movement
        movementDirection = playerCharacter.transform.rotation * movementDirection;
        // Ignore any elevation in the current heading
        movementDirection.y = 0;
        movementDirection = movementDirection.normalized;
        if (xMagnitude != 0.0f || zMagnitude != 0.0f) {
            playerCharacter.moveInDirection(movementDirection);
        }
    }

    private void manageInventory() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            this.playerCharacter.changeHeld(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            this.playerCharacter.changeHeld(1);
        }
    }

    private void manageGunShooting() {
        if (Input.GetMouseButton(0)) {
            this.playerCharacter.useHeld();
        }
    }
}
