using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayerAttachment {
    
    private int[] hotbarMappings;
    private int currentlyHeld;
    public InventoryPlayerAttachment() {
        this.hotbarMappings = new int[10];
        foreach (int num in this.hotbarMappings) {
            this.hotbarMappings[num] = -1;
        }
        this.currentlyHeld = 1;
    }

    public void assignNewHeld(int newHeld) {
        this.currentlyHeld = newHeld;
    }
}
