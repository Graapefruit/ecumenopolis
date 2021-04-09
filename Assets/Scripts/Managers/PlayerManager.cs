using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Manager {

    private PlayerCharacter playerCharacter;

    public PlayerManager(GameObject pcPrefab, GameObject playerControllerPrefab) {
        playerCharacter = (GameObject.Instantiate(pcPrefab, new Vector3 (24.0f, 10.0f, 13.0f), Quaternion.identity)).GetComponent<PlayerCharacter>();
        PlayerController newController = (GameObject.Instantiate(playerControllerPrefab, Vector3.zero, Quaternion.identity)).GetComponent<PlayerController>();
        newController.assignPlayer(playerCharacter);
    }

    public override void onUpdate() {}

    public PlayerCharacter getPlayer() {
        return this.playerCharacter;
    }
}
