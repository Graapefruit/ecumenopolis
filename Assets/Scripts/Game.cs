using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject pmObject;
    private PlayerCharacter player;
    private PlayerController playerController;
    public static Game game;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (game != null) {
            Debug.LogWarning("Static game object already found! Deleting and making another");
            GameObject.Destroy(game);
        }
        game = this;
        game.playerController = pmObject.GetComponent<PlayerController>();
        game.player = game.playerController.getPlayer();
    }

    public static PlayerCharacter getPlayer() {
        if (game.player != null) {
            return game.player.gameObject.GetComponent<PlayerCharacter>();
        } else {
            return null;
        }
    }
}
