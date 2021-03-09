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
        this.hotbarHud = HudManager.getNewHotbarHudInstance();
        // TODO: Calling this causes the hotbar square to vanish off the screen. Likely has to with 
        // this.switchHeld(0);
    }
    public HotbarHudPanel getHotbarHud() {
        return this.hotbarHud;
    }

    public void assignMapping(int x, int y, int h) {
        if (this.inverseHotbarMappings[x, y] == h) {
            return;
        }
        if (this.inverseHotbarMappings[x, y] != -1) {
            this.removeMapping(x, y);
        }

        Item item = this.inventory[x, y];
        if (item != null) {
            if (this.hotbarMappings[h] != null) {
                this.removeMapping(h);
            }
            this.hotbarMappings[h] = new Pair(x, y);
            this.inverseHotbarMappings[x, y] = h;
            this.hud.makeHotbarAssignment(x, y, h);
            this.hotbarHud.setHotbarImage(item, h);
        }
    }

    public void removeMapping(int x, int y) {
        int h = this.inverseHotbarMappings[x, y];
        this.inverseHotbarMappings[x, y] = -1;
        this.hotbarMappings[h] = null;
        this.hud.removeHotbarAssignment(x, y);
        this.hotbarHud.removeHotbarAssignment(h);
    }

    public void removeMapping(int h) {
        Pair pair = this.hotbarMappings[h];
        this.inverseHotbarMappings[pair.x, pair.y] = -1;
        this.hotbarMappings[h] = null;
        this.hud.removeHotbarAssignment(pair.x, pair.y);
        this.hotbarHud.removeHotbarAssignment(h);
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
        this.hotbarHud.CurrentlyHeld = h;
        Pair coords = this.hotbarMappings[h];
        if (coords == null) {
            return null;
        }
        return this.inventory[coords.x, coords.y];
    }

    public Item getHeld() {
        Pair coords = this.hotbarMappings[this.currentlyHeld];
        if (coords == null) {
            return null;
        }
        return this.inventory[coords.x, coords.y];
    }

    public int getHeldIndex() {
        return this.currentlyHeld;
    }

    public override Item pop(int x, int y) {
        Item item = base.pop(x, y);
        if (item != null && this.inverseHotbarMappings[x, y] != -1) {
            this.removeMapping(x, y);
        }
        return item;
    }
}
