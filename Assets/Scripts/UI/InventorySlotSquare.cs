using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void hotbarMapPartialDelegate(int h);
public delegate Item popDelegate();
public delegate Item placeItemDelegate(Item item);

public class InventorySlotSquare : MonoBehaviour {
    // Stores the Inventory's hotbar mapping method as a Partial Application function
    private hotbarMapPartialDelegate partialMapHotbar;
    private popDelegate partialPop;
    private placeItemDelegate partialPlaceItem;

    public void assignPartialFunctions(hotbarMapPartialDelegate h, popDelegate p, placeItemDelegate pl) {
        this.partialMapHotbar = h;
        this.partialPop = p;
        this.partialPlaceItem = pl;
    }

    public void assignHotbarMapping(int h) {
        this.partialMapHotbar(h);
    }

    public Item pop() {
        return this.partialPop();
    }

    public Item placeItem(Item item) {
        return this.partialPlaceItem(item);
    }
}
