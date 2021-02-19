using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    private Item[,] inventory;
    private int inventorySizeX = 6;
    private int inventorySizeY = 6;
    public Inventory() {
        this.inventory = new Item[inventorySizeY, inventorySizeX];
    }

    public void Add(Item item, int x, int y) {

    }

    public Vector2 getDimensions() {
        return new Vector2(inventorySizeX, inventorySizeY);
    }
}
