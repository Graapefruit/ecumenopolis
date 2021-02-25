using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory {
    private Pair[] hotbarMappings;
    private int currentlyHeld;
    private HotbarHudPanel hotbarHud;

    public PlayerInventory() : base() {
        this.hotbarMappings = new Pair[10];
        // for(int i = 0; i < 10; i++) {
        //     this.hotbarMappings[i] = new Pair();
        // }
        this.currentlyHeld = 1;
        this.hotbarHud = HudManager.getNewHotbarHudInstance();
    }
    public HotbarHudPanel getHotbarHud() {
        return this.hotbarHud;
    }

    public void assignMapping(int x, int y, int h) {
        Item item = this.inventory[x, y];
        if (item != null) {
            this.hotbarMappings[h] = new Pair(x, y);
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
