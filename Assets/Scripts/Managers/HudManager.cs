using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HudManager
{
    private Text ammoDisplay;
    private Text heldDisplay;
    private Image hpFill;
    private Image hpTip;
    private int hp;
    private int ammo;
    private string held;
    public HudManager(GameObject hud) {
        this.ammoDisplay = hud.transform.Find("AmmoDisplay").GetComponent<Text>();
        this.heldDisplay = hud.transform.Find("CurrentHeldDisplay").GetComponent<Text>();
        this.hpFill = hud.transform.Find("HpFill").GetComponent<Image>();
        this.hpTip = hud.transform.Find("HpTip").GetComponent<Image>();
        this.updateHud(0, 0, "none");
    }
    public void updateHud(int newHp, int newAmmo, string newHeld) {
        if (this.hp != newHp) {
            this.updateHp(newHp);
        } if (this.ammo != newAmmo) {
            this.updateAmmo(newAmmo);
        } if (this.held != newHeld) {
            this.updateCurrentHeld(newHeld);
        }
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
