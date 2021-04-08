using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Manager {
    public GameObject pcPrefab;
    public GameObject playerControllerPrefab;

    private PlayerCharacter playerCharacter;

    public override void onAwake() {
        playerCharacter = ((GameObject) Instantiate(pcPrefab, new Vector3 (24.0f, 10.0f, 13.0f), Quaternion.identity)).GetComponent<PlayerCharacter>();
    }
    public override void onUpdate()
    {
        throw new System.NotImplementedException();
    }
}
