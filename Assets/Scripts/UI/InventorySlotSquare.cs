using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void hotbarMapPartialDelegate(int h);

public class InventorySlotSquare : MonoBehaviour {
    // Stores the Inventory's hotbar mapping method as a Partial Application function
    private hotbarMapPartialDelegate partialMapHotbar;

    public void assignPartialHotbarMappingFunction(hotbarMapPartialDelegate p) {
        this.partialMapHotbar = p;
    }

    public void assignHotbarMapping(int h) {
        this.partialMapHotbar(h);
    }
}
