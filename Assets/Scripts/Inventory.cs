using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: State for "player attachement"? Would contain hud, and currentlyheld
// TODO: Remove Monobehaviour
public class Inventory {
    private Item[,] inventory;
    private int[] hotbarMappings;
    private Item currentlyHeld;
    private int inventorySizeX = 6;
    private int inventorySizeY = 6;
    private InventoryHudPanel hud;
    public Inventory() {
        this.inventory = new Item[inventorySizeX, inventorySizeY];
        this.hotbarMappings = new int[10];
        foreach (int num in this.hotbarMappings) {
            this.hotbarMappings[num] = -1;
        }
    }

    public InventoryHudPanel getHud() {
        if (this.hud == null) {
            this.createHud();
        }
        return this.hud;
    }

    private void createHud() {
        this.hud = GameObject.Instantiate(HudManager.getInventoryHudManagerPrefab(), Vector3.zero, Quaternion.identity).GetComponent<InventoryHudPanel>();
        this.hud.initializeInventoryHud(this);
        for (int x = 0; x < this.inventorySizeX; x++) {
            for (int y = 0; y < this.inventorySizeY; y++) {
                if (this.inventory[x, y] != null) {
                    this.hud.add(this.inventory[x, y], x, y);
                }
            }
        }
        for (int i = 0; i < 10; i++) {
            // this.hud.setHotbarImage(this.getHotbarAt(i), i);
        }
    }

    public void add(Item item, int x, int y) {
        this.inventory[x, y] = item;
        if (hud != null) {
            this.hud.add(item, x, y);
        }
    }

    // public void setHotbarMapping(int x, int y, int h) {
    //     this.hotbarMappings[h] = (x * this.inventorySizeY) + y;
    //     if (hud != null) {
    //         this.hud.setHotbarImage(this.getHotbarAt(h), h);
    //     }
    // }

    // public Item getHotbarAt(int h) {
    //     int x = this.hotbarMappings[h] / this.inventorySizeY;
    //     int y = this.hotbarMappings[h] % this.inventorySizeY;
    //     return this.inventory[x, y];
    // }

    public Item switchHeld(int h) {
        // this.currentlyHeld = this.getHotbarAt(h);
        return this.currentlyHeld;
    }

    public Vector2 getDimensions() {
        return new Vector2(inventorySizeX, inventorySizeY);
    }
}
