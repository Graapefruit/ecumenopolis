using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory {
    private Pair[] hotbarMappings;
    private int[,] inverseHotbarMappings;
    private int currentlyHeld;
    private HotbarHudPanel hotbarHud;

    public PlayerInventory() : base() {
        this.hotbarMappings = new Pair[10];
        this.inverseHotbarMappings = new int[this.inventorySizeX, this.inventorySizeY];
        for(int x = 0; x < this.inventorySizeX; x++) {
            for (int y = 0; y < this.inventorySizeY; y++) {
                this.inverseHotbarMappings[x, y] = -1;
            }
        }
        this.currentlyHeld = 1;
        this.hotbarHud = HudManager.getNewHotbarHudInstance();
    }
    public HotbarHudPanel getHotbarHud() {
        return this.hotbarHud;
    }

    public void assignMapping(int x, int y, int h) {
        if (this.hotbarMappings[h] != null && this.inverseHotbarMappings[x, y] == h) {
            return;
        }

        Item item = this.inventory[x, y];
        if (item != null) {
            this.hotbarMappings[h] = new Pair(x, y);
            this.inverseHotbarMappings[x, y] = h;
            this.hud.makeHotbarAssignment(x, y, h);
            this.hotbarHud.setHotbarImage(item, h);
        }
    }

    public Item getHotbarAt(int h) {
        Pair coords = this.hotbarMappings[h];
        if (coords == null) {
            return null;
        }
        return this.inventory[coords.x, coords.y];
    }

    public Item switchHeld(int h) {
        this.currentlyHeld = h;
        Pair coords = this.hotbarMappings[h];
        if (coords == null) {
            return null;
        }
        return this.inventory[coords.x, coords.y];
    }
}