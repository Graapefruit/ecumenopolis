using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PlayerHud : MonoBehaviour {
    private Text ammoDisplay;
    private Text heldDisplay;
    private Text pickupPopup;
    private Image hpFill;
    private Image hpTip;
    private InventoryHudPanel inventoryOverlay;
    private HotbarHudPanel hotbarOverlay;
    private int hp;
    private int ammo;
    private string held;
    private PlayerCharacter playerCharacter;

    void Awake() {
        this.ammoDisplay = this.transform.Find("AmmoDisplay").GetComponent<Text>();
        this.heldDisplay = this.transform.Find("CurrentHeldDisplay").GetComponent<Text>();
        this.pickupPopup = this.transform.Find("PickupPopup").GetComponent<Text>();
        this.hpFill = this.transform.Find("HpFill").GetComponent<Image>();
        this.hpTip = this.transform.Find("HpTip").GetComponent<Image>();
    }

    void Update() {
        int newHp = this.playerCharacter.getHp();
        int newAmmo = this.playerCharacter.getCurrentWeaponAmmo();
        string newHeld = this.playerCharacter.getCurrentWeaponName();
        if (this.hp != newHp) {
            this.updateHp(newHp);
        } if (this.ammo != newAmmo) {
            this.updateAmmo(newAmmo);
        } if (this.held != newHeld) {
            this.updateCurrentHeld(newHeld);
        }
    }

    public void setPickupTextEnabled(bool enabled) {
        this.pickupPopup.gameObject.SetActive(enabled);
    }

    public void assignPlayer(PlayerCharacter pc) {
        this.playerCharacter = pc;
        this.inventoryOverlay = pc.getInventoryHud();
        this.hotbarOverlay = pc.getHotbarHud();
        this.inventoryOverlay.transform.SetParent(this.transform, false);
        this.hotbarOverlay.transform.SetParent(this.transform, false);
    }

    public bool toggleInventory() {
        return this.inventoryOverlay.toggleInventory();
    }

    private void updateHp(int newHp) {
        this.hp = newHp;
        Vector3 oldFillPos = this.hpFill.transform.position;
        Vector3 oldFillScale = this.hpFill.transform.localScale;
        Vector3 oldTipPos = this.hpTip.transform.position;
        oldFillPos.x = 105 + (0.85f * newHp);
        oldFillScale.x = 1.8f * newHp;
        oldTipPos.x = 105 + (1.8f * newHp);
        this.hpFill.transform.position = oldFillPos;
        this.hpFill.transform.localScale = oldFillScale;
        this.hpTip.transform.position = oldTipPos;
    }

    public void updateAmmo(int newAmount) {
        this.ammo = newAmount;
        this.ammoDisplay.text = string.Format("Ammo: {0}", newAmount);
    }

    public void updateCurrentHeld(string newHeld) {
        this.held = newHeld;
        this.heldDisplay.text = string.Format("Held: {0}", newHeld);
    }
}
