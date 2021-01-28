using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HudManager : MonoBehaviour
{
    private static HudManager hud;
    void Awake() {
        if (hud != null) {
            Debug.LogWarning("Static HUD object already found! Deleting and making another");
            GameObject.Destroy(hud);
        }
        hud = this;
    }

    public static void updateHealth(int newAmount) {
        Text healthDisplay = hud.transform.Find("HealthDisplay").GetComponent<Text>();
        healthDisplay.text = string.Format("Health: {0}", newAmount);
    }

    public static void updateAmmo(int newAmount) {
        Text ammoDisplay = hud.transform.Find("AmmoDisplay").GetComponent<Text>();
        ammoDisplay.text = string.Format("Ammo: {0}", newAmount);
    }

    public static void updateCurrentHeld(string newHeld) {
        Text heldDisplay = hud.transform.Find("CurrentHeldDisplay").GetComponent<Text>();
        heldDisplay.text = string.Format("Held: {0}", newHeld);
    }
}
