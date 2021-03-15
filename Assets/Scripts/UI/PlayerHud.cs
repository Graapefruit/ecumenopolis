using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PlayerHud : MonoBehaviour {
    private const float STAMINA_END = 105.5f;
    private const float STAMINA_SCALE_MAX = 160.0f;
    private Text ammoDisplay;
    private Text heldDisplay;
    private Text pickupPopup;
    private Image hpFill;
    private Image hpTip;
    private Image staminaFill;
    private Image staminaTip;
    private InventoryHudPanel inventoryOverlay;
    private HotbarHudPanel hotbarOverlay;
    private PlayerCharacter playerCharacter;
    private int lastHp;
    private float lastStamina;

    void Awake() {
        this.ammoDisplay = this.transform.Find("AmmoDisplay").GetComponent<Text>();
        this.heldDisplay = this.transform.Find("CurrentHeldDisplay").GetComponent<Text>();
        this.pickupPopup = this.transform.Find("PickupPopup").GetComponent<Text>();
        this.hpFill = this.transform.Find("HpFill").GetComponent<Image>();
        this.hpTip = this.transform.Find("HpTip").GetComponent<Image>();
        this.staminaFill = this.transform.Find("StaminaFill").GetComponent<Image>();
        this.staminaTip = this.transform.Find("StaminaTip").GetComponent<Image>();
    }

    public void assignPlayer(PlayerCharacter pc) {
        this.playerCharacter = pc;
        PlayerInventory inventory = pc.getInventory();
        this.inventoryOverlay = inventory.getHud();
        this.inventoryOverlay.transform.SetParent(this.transform, false);
        this.hotbarOverlay = inventory.getHotbarHud();
        this.hotbarOverlay.transform.SetParent(this.transform, false);
    }

    void Update() {
        updateHp();
        updateStamina();
        updateHeld();
    }

    public void setPickupTextEnabled(bool enabled) {
        this.pickupPopup.gameObject.SetActive(enabled);
    }

    public bool toggleInventory() {
        return this.inventoryOverlay.toggleInventory();
    }

    public void updateHp() {
        int newHp = this.playerCharacter.getHp();
        if (this.lastHp == newHp) {
            return;
        } 
        this.lastHp = newHp;
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

    public void updateStamina() {
        float newStamina = this.playerCharacter.getStamina();
        if (this.lastStamina == newStamina) {
            return;
        }
        this.lastStamina = newStamina;
        Vector3 oldFillPos = this.staminaFill.transform.position;
        Vector2 oldFillScale = this.staminaFill.rectTransform.sizeDelta;
        Vector3 oldTipPos = this.staminaTip.transform.position;
        float staminaRatio = (newStamina / PlayerCharacter.MAX_STAMINA);
        oldFillPos.x = STAMINA_END + (staminaRatio * (STAMINA_SCALE_MAX / 2));
        oldFillScale.x = staminaRatio * STAMINA_SCALE_MAX;
        oldTipPos.x = STAMINA_END + (staminaRatio * STAMINA_SCALE_MAX);
        this.staminaFill.transform.position = oldFillPos;
        this.staminaFill.rectTransform.sizeDelta = oldFillScale;
        this.staminaTip.transform.position = oldTipPos;
        
    }

    public void updateHeld() {
        Item heldItem = this.playerCharacter.getInventory().getHeld();
        if (heldItem == null) {
            this.ammoDisplay.gameObject.SetActive(false);
            this.heldDisplay.text = "Unarmed";
        } else if (heldItem is Gun) {
            this.ammoDisplay.gameObject.SetActive(true);
            this.heldDisplay.text = heldItem.name;
            this.ammoDisplay.text = string.Format("Ammo: {0}", ((Gun) heldItem).getRemainingAmmo());
        } else {
            this.ammoDisplay.gameObject.SetActive(false);
            this.heldDisplay.text = heldItem.name;
        }
    }
}
