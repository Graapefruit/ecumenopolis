using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void hotbarMapPartialDelegate(int h);
public delegate Item popDelegate();

public class InventorySlotSquare : MonoBehaviour {
    // Stores the Inventory's hotbar mapping method as a Partial Application function
    private hotbarMapPartialDelegate partialMapHotbar;
    private popDelegate partialPop;

    public void assignPartialFunctions(hotbarMapPartialDelegate h, popDelegate p) {
        this.partialMapHotbar = h;
        this.partialPop = p;
    }

    public void assignHotbarMapping(int h) {
        this.partialMapHotbar(h);
    }

    public Item pop() {
        return this.partialPop();
    }
}
