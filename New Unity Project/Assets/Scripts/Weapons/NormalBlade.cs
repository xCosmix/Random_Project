using UnityEngine;
using System.Collections;
using ObjectPool;

public class NormalBlade : Weapon
{
    protected GameObject slash;
    protected GameObject overChargedSlash;

    public NormalBlade(Entity owner) : base(owner)
    {
        slash = Resources.Load<GameObject>("Prefabs/Slash01");
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
            return 15.0f;
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
        GameObject instance = Pool.New(slash, (Vector2)owner.transform.position + owner.AimDir * 0.3f, Quaternion.LookRotation(Vector3.forward, owner.AimDir), 20);
        Blade bladeInstance = instance.GetComponent<Blade>();
        bladeInstance.New(direction, this);
    }
}

