using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: State for "player attachement"? Would contain hud, and currentlyheld
// TODO: Remove Monobehaviour
public class Inventory {
    private Item[,] inventory;
    private int inventorySizeX = 6;
    private int inventorySizeY = 6;
    private InventoryHudPanel hud;
    private InventoryPlayerAttachment playerAttachment;

    public Inventory() {
        this.inventory = new Item[inventorySizeX, inventorySizeY];
    }

    public InventoryHudPanel getHud() {
        if (this.hud == null) {
            this.initializeHud();
        }
        return this.hud;
    }

    private void initializeHud() {
        this.hud = HudManager.getNewInventoryHudInstance();
        this.hud.initializeInventoryHud(this);
        for (int x = 0; x < this.inventorySizeX; x++) {
            for (int y = 0; y < this.inventorySizeY; y++) {
                if (this.inventory[x, y] != null) {
                    this.hud.add(this.inventory[x, y], x, y);
                }
            }
        }
    }

    public void add(Item item, int x, int y) {
        this.inventory[x, y] = item;
        if (hud != null) {
            this.hud.add(item, x, y);
        }
    }

    public Pair getNextOpenSlot() {
        for (int y = 0; y < this.inventorySizeY; y++) {
            for (int x = 0; x < this.inventorySizeX; x++) {
                if (this.inventory[x, y] == null) {
                    return new Pair(x, y);
                }
            }
        }
        return new Pair(-1, -1);
    }

    public void assignMapping(int x, int y, int h) {
        Item item = this.inventory[x, y];
        this.playerAttachment.assignMapping(x, y, h, item);
    }

    public Item getHotbarAt(int h) {
        Pair coords = this.playerAttachment.getMapping(h);
        return this.inventory[coords.x, coords.y];
    }

    public HotbarHudPanel getHotbarHud() {
        if (this.playerAttachment == null) {
            this.playerAttachment = new InventoryPlayerAttachment();
        } 
        return this.playerAttachment.getHotbarHud();
    }

    public Item switchHeld(int h) {
        if (this.playerAttachment.assignNewHeld(h)) {
            Pair pair = this.playerAttachment.getMapping(h);
            return this.inventory[pair.x, pair.y];
        } else {
            return null;
        }
    }

    public Pair getDimensions() {
        return new Pair(inventorySizeX, inventorySizeY);
    }
}
