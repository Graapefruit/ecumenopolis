using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwivelHelper {
    private const float SWIVEL_DURATION = 0.4f;
    private float heading;
    private float feetDirection;
    private bool swivelingLeft;
    private bool swivelingRight;
    private float swivelTimeRemaining;
    private Animator animator;

    public PlayerSwivelHelper(Animator animator) {
        this.animator = animator;
        this.heading = 0.0f;
        this.feetDirection = 0.0f;
        this.swivelingLeft = false;
        this.swivelingRight = false;
        this.swivelTimeRemaining = 0.0f;
    }

    public void manageSwivel(float heading, float feetDirection) {
        this.heading = heading;
        this.feetDirection = feetDirection;
        float contortionAngle = this.calculateContortion();

        if (contortionAngle < 270 && contortionAngle > 180) {
            this.animator.SetInteger("turnDirection", 1);
            this.swivelingRight = true;
            this.swivelTimeRemaining = SWIVEL_DURATION;
        } else if (contortionAngle > 90 && contortionAngle < 180) {
            this.animator.SetInteger("turnDirection", 2);
            this.swivelingLeft = true;
            this.swivelTimeRemaining = SWIVEL_DURATION;
        } else if (this.isSwiveling()) {
            this.swivelTimeRemaining -= Time.deltaTime * (1.0f / SWIVEL_DURATION);
            if (this.swivelTimeRemaining <= 0.0f) {
                this.stopSwiveling();
            }
        } else {
            this.stopSwiveling();
        }
    }

    public float getSwivelAmountWithoutDeltaTime() {
        return 90.0f * (1.0f / SWIVEL_DURATION) * (this.isSwiveling() ? (this.swivelingRight ? 1 : -1) : 0);
    }

    public void stopSwiveling() {
        this.animator.SetInteger("turnDirection", 0);
        this.swivelingLeft = false;
        this.swivelingRight = false;
        this.swivelTimeRemaining = 0.0f;
    }

    private bool isSwiveling() {
        return this.swivelingRight || this.swivelingLeft;
    }

    private float calculateContortion() {
        float contortionAngle = this.feetDirection - this.heading;
        while (contortionAngle < 0) {
            contortionAngle += 360;
        }
        return contortionAngle;
    }
}
