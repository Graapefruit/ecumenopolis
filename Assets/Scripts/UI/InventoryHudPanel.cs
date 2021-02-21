using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryHudPanel : MonoBehaviour{
    public GameObject itemAvatarPrefab;
    public GameObject inventorySquarePrefab;
    private int inventorySizeX;
    private int inventorySizeY;
    private GameObject[,] inventoryItemSquares;
    private GameObject[,] inventoryItemImages;
    private Inventory inventory;
    private bool isActive;

    public void initializeInventoryHud(Inventory inventory) {
        this.inventory = inventory;
        Vector2 gridSize = this.inventory.getDimensions();
        this.inventorySizeX = (int) gridSize.x;
        this.inventorySizeY = (int) gridSize.y;
        this.inventoryItemImages = new GameObject[inventorySizeX, inventorySizeY];
        this.inventoryItemSquares = new GameObject[inventorySizeX, inventorySizeY];
        RectTransform panelRectTransform = this.transform.GetComponent<RectTransform>();
        panelRectTransform.anchorMin = new Vector2(1, 0);
        panelRectTransform.anchorMax = new Vector2(0, 1);
        panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
        this.initializeInventoryPanels();
        this.isActive = true;
        this.toggleInventory();
    }

    public void add(Item item, int x, int y) {
        Vector2 coords = this.indexToCoords(x, y);
        float xPos = coords.x;
        float yPos = coords.y;
        GameObject itemAvatar = Instantiate(itemAvatarPrefab, new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
        itemAvatar.transform.SetParent(this.transform, false);
        this.inventoryItemImages[x, y] = itemAvatar;
        itemAvatar.GetComponent<Image>().sprite = item.getAvatar();
    }

    public void setHotbarImage(Item item, int h) {

    }

    public bool toggleInventory() {
        isActive = !isActive;
        this.gameObject.SetActive(isActive);
        return isActive;
    }

    private void initializeInventoryPanels() {
        for(int x = 0; x < this.inventorySizeX; x++) {
            for(int y = 0; y < this.inventorySizeY; y++) {
                Vector2 coords = this.indexToCoords(x, y);
                GameObject newSquare = Instantiate(inventorySquarePrefab, new Vector3(coords.x, coords.y, 0.0f), Quaternion.identity);
                newSquare.transform.SetParent(this.transform, false);
                this.inventoryItemSquares[x, y] = newSquare;
            }
        }
        RectTransform panel = this.GetComponent<RectTransform>();
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(20 + (60 * this.inventorySizeX), 20 + (60 * this.inventorySizeY));
    }

    private Vector2 indexToCoords(int x, int y) {
        return new Vector2((-30 * (this.inventorySizeX - 1)) + (60 * x), (30 * (this.inventorySizeY - 1)) + (-60 * y));
    }
}
