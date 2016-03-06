using UnityEngine;
using System.Collections;
using System;
using ObjectPool;
[System.Serializable]
public class NormalCannon : Weapon
{
    protected GameObject bullet;
    protected GameObject overChargeBullet;

    public NormalCannon(Entity owner) : base(owner)
    {
        bullet = Resources.Load<GameObject>("Prefabs/Bullet01");
    }

    public override float ColdDown
    {
        get
        {
            return 0.2f;
        }
    }

    public override float HitWeight
    {
        get
        {
            return 5.0f;
        }
    }

    public override float OverChargeValue
    {
        get
        {
            return 50.0f;
        }
    }

    public override void OverCharge()
    {
        Debug.Log("bum");
    }

    public override void Shoot()
    {
        GameObject instance = Pool.New(bullet, (Vector2)owner.transform.position + owner.AimDir, bullet.transform.rotation, 20);
        Bullet bulletInstance = instance.GetComponent<Bullet>();
        bulletInstance.New(direction, this);
    }
}
