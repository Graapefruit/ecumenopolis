using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public GameObject pcPrefab;
    public GameObject hudPrefab;
    private PlayerHud hud;
    private PlayerCharacter playerCharacter;

    void Awake() {
        this.playerCharacter = ((GameObject) Instantiate(pcPrefab, new Vector3 (34.0f, 11.5f, 3.0f), Quaternion.identity)).GetComponent<PlayerCharacter>();
        this.hud = ((GameObject) Instantiate(hudPrefab, Vector3.zero, Quaternion.identity)).GetComponent<PlayerHud>();
    }

    void Start() {
        this.hud.assignPlayer(this.playerCharacter);
    }

    void Update() {
        manageCameraLocking();
        if (Cursor.lockState == CursorLockMode.Locked) {
            manageMouseMovements();
            manageMovement();
            manageHotbar();
            manageGunShooting();
            manageInventoryOpenClose();
        }
    }

    public PlayerCharacter getPlayer() {
        return this.playerCharacter;
    } 

    private void manageCameraLocking() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
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
        if (xMagnitude != 0.0f || zMagnitude != 0.0f) {
            Vector3 movementDirection = new Vector3(xMagnitude, 0.0f, zMagnitude).normalized;
            movementDirection = playerCharacter.transform.rotation * movementDirection;
            movementDirection.y = 0;
            movementDirection = movementDirection.normalized;
            this.playerCharacter.setMovementDirection(movementDirection);
        }

        if (Input.GetKey(KeyCode.Space)) {
            this.playerCharacter.jump();
        }
    }

    private void manageHotbar() {
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

    private void manageInventoryOpenClose() {
        if (Input.GetKeyDown(KeyCode.I)) {
            this.hud.toggleInventory();
        }
    }
}
