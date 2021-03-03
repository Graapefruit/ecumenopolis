using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {
    public GameObject pcPrefab;
    public GameObject hudPrefab;
    private PlayerCharacter playerCharacter;
    private PlayerHud hud;
    private ItemDragHelper itemDragHelper;
    private PlayerInventory playerInventory;
    private bool inventoryIsOpen;

    void Awake() {
        this.playerCharacter = ((GameObject) Instantiate(pcPrefab, new Vector3 (34.0f, 11.5f, 3.0f), Quaternion.identity)).GetComponent<PlayerCharacter>();
        this.hud = ((GameObject) Instantiate(hudPrefab, Vector3.zero, Quaternion.identity)).GetComponent<PlayerHud>();
        this.itemDragHelper = this.hud.transform.Find("DraggedItem").GetComponent<ItemDragHelper>();
        this.inventoryIsOpen = false;
    }

    void Start() {
        this.playerCharacter.finishInitialization();
        this.playerInventory = this.playerCharacter.getInventory();
        this.hud.assignPlayer(this.playerCharacter);
    }

    void Update() {
        checkForInterfaceUpdates();
        if (this.inventoryIsOpen) {
            manageHotbarAssignments();
            manageDragging();
            manageDropping();
        } else if (cameraIsLocked()) {
            manageMouseMovements();
            manageMovement();
            manageHotbar();
            manageGunShooting();
            manageItemPickup();
        }
    }

    public PlayerCharacter getPlayer() {
        return this.playerCharacter;
    } 

    private void checkForInterfaceUpdates() {
        if (Input.GetKeyDown(KeyCode.I)) {
            this.hud.toggleInventory();
            this.inventoryIsOpen = !this.inventoryIsOpen;
            Cursor.lockState = (this.inventoryIsOpen ? CursorLockMode.None : CursorLockMode.Locked);
            this.itemDragHelper.gameObject.SetActive(false);
            if (this.itemDragHelper.hasItem()) {
                PickupManager.dropItem(this.playerCharacter.transform.position, this.itemDragHelper.releaseItem());
            }
        } if (Input.GetMouseButton(0) && !this.inventoryIsOpen) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void manageHotbarAssignments() {
        int hotbarSlot = this.getHotbarSlotDown();
        if (hotbarSlot < 0) {
            return;
        }
        InventorySlotSquare square = this.getItemButtonHovered();
        if (square != null) {
            square.assignHotbarMapping(hotbarSlot);
            if (this.playerInventory.getHeldIndex() == hotbarSlot) {
                this.playerCharacter.changeHeld(hotbarSlot);
            }
        }
    }

    private void manageDragging() {
        if (this.itemDragHelper.hasItem() && Input.GetMouseButtonUp(0)) {
            InventorySlotSquare square = this.getItemButtonHovered();
            Item releasedItem = this.itemDragHelper.releaseItem();
            if (square != null) {
                Item swappedItem = square.placeItem(releasedItem);
                if (swappedItem != null) {
                    this.itemDragHelper.dragItem(swappedItem);
                }
            } else {
                PickupManager.dropItem(this.playerCharacter.transform.position, releasedItem);
            }
        } else if (!this.itemDragHelper.hasItem() && Input.GetMouseButtonDown(0)) {
            InventorySlotSquare square = this.getItemButtonHovered();
            if (square != null) {
                Item poppedItem = square.pop();
                if (poppedItem != null) {
                    this.itemDragHelper.dragItem(poppedItem);
                }
            } 
        }
    }

    private void manageDropping() {
        if (Input.GetKeyDown("q")) {
            InventorySlotSquare square = this.getItemButtonHovered();
            if (square == null) {
                return;
            }
            PickupManager.dropItem(this.playerCharacter.transform.position, square.pop());
        }
    }

    private InventorySlotSquare getItemButtonHovered() {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        foreach (RaycastResult result in results) {
            InventorySlotSquare square = result.gameObject.GetComponent<InventorySlotSquare>();
            if (square != null) {
                return square;
            }
        }
        return null;
    }

    private int getHotbarSlotDown() {
        // ASCII
        for (int i = 48; i < 58; i++) {
            if (Input.GetKeyDown(""+((char) i))) {
                return i - 48;
            }
        }
        return -1;
    }

    private bool cameraIsLocked() {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    private void manageMouseMovements() {
        float mouseDeltaX = Input.GetAxis("Mouse X");
        float mouseDeltaY = Input.GetAxis("Mouse Y");
        if (mouseDeltaX != 0 || mouseDeltaY != 0) {
            playerCharacter.changeLookDirection(mouseDeltaX, mouseDeltaY);
        }
    }

    private void manageMovement() {
        float xMagnitude = 0.0f;
        float zMagnitude = 0.0f;
        xMagnitude += (Input.GetKey("d") ? 1.0f : 0.0f);
        xMagnitude += (Input.GetKey("a") ? -1.0f : 0.0f);
        zMagnitude += (Input.GetKey("w") ? 1.0f : 0.0f);
        zMagnitude += (Input.GetKey("s") ? -1.0f : 0.0f);
        if (xMagnitude != 0.0f || zMagnitude != 0.0f) {
            Vector3 movementDirection = new Vector3(xMagnitude, 0.0f, zMagnitude).normalized;
            movementDirection = playerCharacter.transform.rotation * movementDirection;
            movementDirection.y = 0;
            movementDirection = movementDirection.normalized;
            this.playerCharacter.setMovementDirection(movementDirection);
        }

        if (Input.GetKey(KeyCode.Space)) {
            this.playerCharacter.jump();
        }
    }

    private void manageHotbar() {
        int hotbarKeyDown = getHotbarSlotDown();
        if (hotbarKeyDown != -1) {
            this.playerCharacter.changeHeld(hotbarKeyDown);
        }
    }

    private void manageGunShooting() {
        if (Input.GetMouseButton(0)) {
            this.playerCharacter.useHeld();
        }
    }

    private void manageItemPickup() {
        Pickup pickup = this.playerCharacter.getFirstPickupInRange();
        if (pickup != null) {
            this.hud.setPickupTextEnabled(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                Item item = PickupManager.pickupItem(pickup);
                bool itemWasAdded = this.playerCharacter.addItemIfRoom(item);
            }
        } else {
            this.hud.setPickupTextEnabled(false);
        }
    }
}
