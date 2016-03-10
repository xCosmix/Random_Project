using UnityEngine;
using System.Collections;
using System;

public class ContactHazard : Weapon {

    private float hitWeight;
    private float radius;

    public ContactHazard(Entity owner, float hitWeight, float radius) : base(owner) { this.hitWeight = hitWeight; this.radius = radius; }

    public override float ColdDown
    {
        get
        {
            return 0.0f;
        }
    }

    public override float HitWeight
    {
        get
        {
            return hitWeight;
        }
    }

    public override float OverChargeValue
    {
        get
        {
            return 0.0f;
        }
    }

    public override void OverCharge() { }

    public override void Shoot()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(owner.transform.position, radius, 512);

        foreach (Collider2D target in targets)
        {
            if (target.gameObject == Player.player.gameObject)
            {
                Player.player.WeaponHit(this, owner.transform.position);
                break;
            }
        }
    }
}
