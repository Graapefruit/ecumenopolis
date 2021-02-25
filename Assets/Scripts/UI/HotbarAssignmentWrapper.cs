using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarAssignmentWrapper : MonoBehaviour {
    private static readonly Color orange = new Color(1.0f, 0.5f, 0.0f, 1.0f);
    private static readonly Color purple = new Color(0.5f, 0.0f, 1.0f, 1.0f);
    private static readonly Color[] indexToColour = {Color.grey, Color.red, Color.blue, Color.yellow, Color.green, orange, purple, Color.cyan, Color.magenta, Color.black};
    private Image itemWrapper;
    private Text hotbarNum;

    void Awake() {
        this.itemWrapper = this.GetComponent<Image>();
        this.hotbarNum = this.transform.GetChild(0).GetComponent<Text>();
        Debug.Log("hi!");
    }

    public void changeAssignment(int i) {
        this.itemWrapper.color = indexToColour[i];
        this.hotbarNum.text = i.ToString();
    }
}
