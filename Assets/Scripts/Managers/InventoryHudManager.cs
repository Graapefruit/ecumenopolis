using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryHudManager : MonoBehaviour{
    public GameObject inventorySquarePrefab;
    private RectTransform inventoryPanel;
    private Inventory inventory;
    private bool isActive;

    void Awake() {
        this.inventoryPanel = this.transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void assignInventory(Inventory inventory) {
        this.inventory = inventory;
        RectTransform panelRectTransform = this.transform.GetComponent<RectTransform>();
        panelRectTransform.anchorMin = new Vector2(1, 0);
        panelRectTransform.anchorMax = new Vector2(0, 1);
        panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
        this.initializeInventoryPanels();
        this.isActive = true;
        this.toggleInventory();
    }

    public bool toggleInventory() {
        isActive = !isActive;
        this.gameObject.SetActive(isActive);
        return isActive;
    }

    private void initializeInventoryPanels() {
        Vector2 gridSize = this.inventory.getDimensions();
        for(float x = (-30 * (gridSize.x - 1)); x <= (30 * (gridSize.x - 1)) ; x += 60) {
            for(float y = (30 * (gridSize.y - 1)); y >= (-30 * (gridSize.y - 1)) ; y -= 60) {
                GameObject newSquare = Instantiate(inventorySquarePrefab, new Vector3(x, y, 0.0f), Quaternion.identity);
                newSquare.transform.SetParent(this.transform, false);
            }
        }
        this.inventoryPanel.sizeDelta = new Vector2(20 + (60 * gridSize.x), 20 + (60 * gridSize.y));
    }
}
