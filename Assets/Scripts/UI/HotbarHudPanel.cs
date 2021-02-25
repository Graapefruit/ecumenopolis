using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarHudPanel : MonoBehaviour {
    public GameObject itemAvatarPrefab;
    public GameObject hotbarSquarePrefab;
    private GameObject[] hotbarSquares;
    private GameObject[] hotbarItemImages;

    void Start() {
        this.hotbarSquares = new GameObject[10];
        this.hotbarItemImages = new GameObject[10];
        for (int i = 0; i < 10; i++) {
            Vector3 coords = this.indexToHotbarCoords(i);
            GameObject newSquare = Instantiate(hotbarSquarePrefab, coords, Quaternion.identity);
            newSquare.transform.SetParent(this.transform, false);
            this.hotbarSquares[i] = newSquare;
        }
    }

    public void setHotbarImage(Item item, int h) {
        Vector3 coords = this.indexToHotbarCoords(h);
        GameObject itemAvatar = Instantiate(itemAvatarPrefab, coords, Quaternion.identity);
        itemAvatar.transform.SetParent(this.transform, false);
        Vector3 relativeItemPos = itemAvatar.transform.position;
        relativeItemPos.y += 20.0f;
        itemAvatar.transform.position = relativeItemPos;
        this.hotbarSquares[h] = itemAvatar;
        itemAvatar.GetComponent<Image>().sprite = item.getAvatar();
    }

    private Vector3 indexToHotbarCoords(int h) {
        return new Vector3(-350.0f + (70.0f * h), -10.0f, 0.0f);
    }

    // TODO: Coroutines to indicate selected hotbar?
}
