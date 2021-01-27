using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime
{
    private readonly Mover recipient;
    private readonly float interval;
    private readonly float stoppingPower;
    private readonly int damage;
    private float nextTime;
    public DamageOverTime(Mover recipient, float dps, float stoppingPower) {
        this.recipient = recipient;
        this.interval = 1 / dps;
        this.stoppingPower = stoppingPower;
        this.nextTime = Time.time;
    }

    public bool stillValid() {
        return recipient;
    }

    public void tick() {
        while (nextTime < Time.time) {
            this.recipient.dealDamage(1, stoppingPower);
            this.nextTime += interval;
        }
    }
}
