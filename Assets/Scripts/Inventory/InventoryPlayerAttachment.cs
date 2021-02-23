using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayerAttachment {
    
    private Pair[] hotbarMappings;
    private int currentlyHeld;
    private HotbarHudPanel hudPanel;

    public InventoryPlayerAttachment() {
        this.hotbarMappings = new Pair[10];
        for(int i = 0; i < 10; i++) {
            this.hotbarMappings[i] = new Pair(-1, -1);
        }
        this.currentlyHeld = 1;
        this.hudPanel = HudManager.getNewHotbarHudInstance();
    }

    public HotbarHudPanel getHotbarHud() {
        return this.hudPanel;
    }

    public bool assignNewHeld(int newHeld) {
        if (this.pairUnassigned(newHeld)) {
            return false;
        } else {
            this.currentlyHeld = newHeld;
            return true;
        }
    }

    public void assignMapping(int x, int y, int h, Item item) {
        this.hotbarMappings[h] = new Pair(x, y);
        this.hudPanel.setHotbarImage(item, h);
    }

    public Pair getMapping(int i) {
        if (this.pairUnassigned(i)) {
            return null;
        } else {
            return this.hotbarMappings[i];
        }
    }

    private bool pairUnassigned(int i) {
        Pair pair = this.hotbarMappings[i];
        return pair.x == -1 || pair.y == -1;
    }
}
