using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarHudPanel : MonoBehaviour {
    private const float HOTBAR_BASE_HEIGHT = -10.0f;
    private const float HOTBAR_SELECTED_HEIGHT = 25.0f;
    private const float HOTBAR_SELECTED_DURATION = 0.2f;
    public GameObject itemAvatarPrefab;
    public GameObject hotbarSquarePrefab;
    private int currentlyHeld;
    public int CurrentlyHeld {
        get { return currentlyHeld; }
        set {
            if (this.hotbarSquaresSelectionCoroutines[currentlyHeld] != null) {
                StopCoroutine(this.hotbarSquaresSelectionCoroutines[currentlyHeld]);
                IEnumerator deselectHotbar = this.deselectHotbar(currentlyHeld);
                this.hotbarSquaresSelectionCoroutines[currentlyHeld] = deselectHotbar;
                StartCoroutine(deselectHotbar);
            }
            if (this.hotbarSquaresSelectionCoroutines[value] != null) {
                StopCoroutine(this.hotbarSquaresSelectionCoroutines[value]);
            }
            IEnumerator selectHotbar = this.selectHotbar(value);
            this.hotbarSquaresSelectionCoroutines[value] = selectHotbar;
            StartCoroutine(selectHotbar);
            this.currentlyHeld = value;
         }
    }
    private GameObject[] hotbarSquares;
    private GameObject[] hotbarItemImages;
    private IEnumerator[] hotbarSquaresSelectionCoroutines;

    void Awake() {
        this.hotbarSquaresSelectionCoroutines = new IEnumerator[10];
        this.hotbarSquares = new GameObject[10];
        this.hotbarItemImages = new GameObject[10];
        for (int i = 0; i < 10; i++) {
            GameObject newSquare = Instantiate(hotbarSquarePrefab, this.indexToHotbarCoords(i), Quaternion.identity);
            newSquare.transform.SetParent(this.transform, true);
            this.hotbarSquares[i] = newSquare;
        }
    }

    public void setHotbarImage(Item item, int h) {
        GameObject itemAvatar = Instantiate(itemAvatarPrefab, new Vector3(0.0f, 20.0f, 0.0f), Quaternion.identity);
        itemAvatar.transform.SetParent(this.hotbarSquares[h].transform, false);
        this.hotbarItemImages[h] = itemAvatar;
        itemAvatar.GetComponent<Image>().sprite = item.avatar;
    }

    public void removeHotbarAssignment(int h) {
        Destroy(this.hotbarItemImages[h]);
        this.hotbarItemImages[h] = null;
    }

    private Vector3 indexToHotbarCoords(int h) {
        return new Vector3(-350.0f + (70.0f * h), -10.0f, 0.0f);
    }

    private IEnumerator selectHotbar(int selected) {
        Vector3 position = this.hotbarSquares[selected].transform.position;
        while (position.y < HOTBAR_SELECTED_HEIGHT) {
            position.y += (HOTBAR_SELECTED_HEIGHT - HOTBAR_BASE_HEIGHT) * Time.deltaTime / HOTBAR_SELECTED_DURATION;
            this.hotbarSquares[selected].transform.position = position;
            yield return null;
        }
        position.y = HOTBAR_SELECTED_HEIGHT;
        this.hotbarSquares[selected].transform.position = position;
    }

    private IEnumerator deselectHotbar(int selected) {
        Vector3 position = this.hotbarSquares[selected].transform.position;
        while (position.y > HOTBAR_BASE_HEIGHT) {
            position.y -= (HOTBAR_SELECTED_HEIGHT - HOTBAR_BASE_HEIGHT) * Time.deltaTime / HOTBAR_SELECTED_DURATION;
            this.hotbarSquares[selected].transform.position = position;
            yield return null;
        }
        position.y = HOTBAR_BASE_HEIGHT;
        this.hotbarSquares[selected].transform.position = position;
    }
}
