using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    protected Item[,] inventory;
    protected int inventorySizeX = 6;
    protected int inventorySizeY = 6;
    protected InventoryHudPanel hud;

    public Inventory() {
        this.inventory = new Item[inventorySizeX, inventorySizeY];
        this.initializeHud();
    }

    public InventoryHudPanel getHud() {
        return this.hud;
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

    public Pair getDimensions() {
        return new Pair(inventorySizeX, inventorySizeY);
    }

    // ----- Helpers -----

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
}
