using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    protected Item[,] inventory;
    protected int inventorySizeX = 6;
    protected int inventorySizeY = 6;
    protected InventoryHudPanel hud;

    public Inventory(InventoryHudPanel hud) {
        this.inventory = new Item[inventorySizeX, inventorySizeY];
        this.hud = hud;
        initializeHud();
    }

    public InventoryHudPanel getHud() {
        return this.hud;
    }

    protected virtual void initializeHud() {
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

    public virtual Item pop(int x, int y) {
        Item item = this.inventory[x, y];
        if (item != null) {
            this.hud.removeItem(x, y);
            this.inventory[x, y] = null;
        }
        return item;
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
}
