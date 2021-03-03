using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemDragHelper : MonoBehaviour {
    private Image image;
    private Item draggedItem;

    void Update() {
        this.transform.position = Input.mousePosition;
    }

    public void dragItem(Item item) {
        this.assignImageIfNull();
        this.draggedItem = item;
        this.image.sprite = item.avatar;
        this.gameObject.SetActive(true);
    }

    public Item releaseItem() {
        this.assignImageIfNull();
        Item toReturn = this.draggedItem;
        this.draggedItem = null;
        this.image.sprite = null;
        this.gameObject.SetActive(false);
        return toReturn;
    }

    public bool hasItem() {
        return this.draggedItem != null;
    }

    private void assignImageIfNull() {
        if (image == null) {
            this.image = this.transform.GetComponent<Image>();
        }
    }
}
