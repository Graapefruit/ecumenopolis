using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryHudPanel : MonoBehaviour{
    public GameObject itemAvatarPrefab;
    public GameObject inventorySquarePrefab;
    public GameObject hotbarAssignmentWrapperPrefab;
    private int inventorySizeX;
    private int inventorySizeY;
    private GameObject[,] inventoryItemSquares;
    private GameObject[,] inventoryItemImages;
    private GameObject[,] hotbarAssignmentWrappers;
    private bool isActive;

    public void initializeInventoryHud(Inventory inventory) {
        Pair gridSize = inventory.getDimensions();
        this.inventorySizeX = gridSize.x;
        this.inventorySizeY = gridSize.y;
        RectTransform panelRectTransform = this.transform.GetComponent<RectTransform>();
        panelRectTransform.anchorMin = new Vector2(1, 0);
        panelRectTransform.anchorMax = new Vector2(0, 1);
        panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
        this.initializeInventoryPanels(inventory);
        this.isActive = true;
        this.toggleInventory();
    }

    public void add(Item item, int x, int y) {
        Vector3 coords = this.indexToInventoryCoords(x, y);
        GameObject itemAvatar = Instantiate(itemAvatarPrefab, coords, Quaternion.identity);
        itemAvatar.transform.SetParent(this.transform, false);
        this.inventoryItemImages[x, y] = itemAvatar;
        itemAvatar.GetComponent<Image>().sprite = item.getAvatar();
    }

    public void makeHotbarAssignment(int x, int y, int h) {
        Vector3 coords = this.indexToInventoryCoords(x, y);
        GameObject hotbarWrapper = Instantiate(hotbarAssignmentWrapperPrefab, coords, Quaternion.identity);
        hotbarWrapper.GetComponent<HotbarAssignmentWrapper>().changeAssignment(h);
        hotbarWrapper.transform.SetParent(this.transform, false);
        this.hotbarAssignmentWrappers[x, y] = hotbarWrapper;
    }

    public void removeHotbarAssignment(int x, int y) {
        Destroy(this.hotbarAssignmentWrappers[x, y]);
        this.hotbarAssignmentWrappers[x, y] = null;
    }

    public bool toggleInventory() {
        isActive = !isActive;
        this.gameObject.SetActive(isActive);
        return isActive;
    }

    private void initializeInventoryPanels(Inventory inventory) {
        this.inventoryItemImages = new GameObject[inventorySizeX, inventorySizeY];
        this.inventoryItemSquares = new GameObject[inventorySizeX, inventorySizeY];
        this.hotbarAssignmentWrappers = new GameObject[inventorySizeX, inventorySizeY];
        for(int x = 0; x < this.inventorySizeX; x++) {
            for(int y = 0; y < this.inventorySizeY; y++) {
                createNewSquare(inventory, x, y);
            }
        }
        RectTransform panel = this.GetComponent<RectTransform>();
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(20 + (60 * this.inventorySizeX), 20 + (60 * this.inventorySizeY));
    }

    private void createNewSquare(Inventory inventory, int x, int y) {
        Vector3 coords = this.indexToInventoryCoords(x, y);
        GameObject newSquare = Instantiate(inventorySquarePrefab, coords, Quaternion.identity);
        if (inventory is PlayerInventory) {
            PlayerInventory playerInventory = inventory as PlayerInventory;
            newSquare.GetComponent<InventorySlotSquare>().assignPartialHotbarMappingFunction(((int h) => { playerInventory.assignMapping(x, y, h); }));
        }
        newSquare.transform.SetParent(this.transform, false);
        this.inventoryItemSquares[x, y] = newSquare;
    }

    private Vector3 indexToInventoryCoords(int x, int y) {
        return new Vector3((-30 * (this.inventorySizeX - 1)) + (60 * x), (30 * (this.inventorySizeY - 1)) + (-60 * y), 0.0f);
    }
}
