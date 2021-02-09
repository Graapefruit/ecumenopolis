using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject ammoPrefab;
    public GameObject pmObject;
    private PlayerCharacter player;
    private const float ammoSpawnHeight = 0.5f;
    private const float ammoBaseCooldown = 3.5f;
    private float remainingAmmoCooldown = 3.5f;
    private PlayerManager playerManager;
    public static Game game;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (game != null) {
            Debug.LogWarning("Static game object already found! Deleting and making another");
            GameObject.Destroy(game);
        }
        game = this;
        game.playerManager = pmObject.GetComponent<PlayerManager>();
        game.player = game.playerManager.getPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        remainingAmmoCooldown = manageSpawnCooldown(ammoPrefab, ammoBaseCooldown, remainingAmmoCooldown);
    }

    private static float manageSpawnCooldown(GameObject gameObject, float baseCooldown, float remainingCooldown) {
        float returnedCooldown = remainingCooldown - Time.deltaTime;
        if (returnedCooldown <= 0.0f) {
            Vector3 spawnLocation = BoardManager.getRandomLocation();
            Instantiate(gameObject, spawnLocation, Quaternion.identity);
            returnedCooldown += baseCooldown;
        }
        return returnedCooldown;
    }

    public static PlayerCharacter getPlayer() {
        if (game.player != null) {
            return game.player.gameObject.GetComponent<PlayerCharacter>();
        } else {
            return null;
        }
    }
}
