using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject playerCharacterPrefab;
    public GameObject playerControllerPrefab;
    private static Game game;
    private PlayerManager playerManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (game != null) {
            Debug.LogWarning("Static game object already found! Deleting and making another");
            GameObject.Destroy(game);
        }
        game = this;
        playerManager = new PlayerManager(playerCharacterPrefab, playerControllerPrefab);
    }

    public static PlayerCharacter getPlayer() {
        return game.playerManager.getPlayer();
    }
}
