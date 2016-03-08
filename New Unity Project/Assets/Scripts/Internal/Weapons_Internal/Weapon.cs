using UnityEngine;

public interface IWeapon {

    float HitWeight
    {
        get;
    }
    float ColdDown
    {
        get;
    }
    float OverChargeValue
    {
        get;
    }

    void Trigger(Vector2 direction);

    void OverCharge();
    void Shoot();

    bool CanUse();
}
[System.Serializable]
public abstract class Weapon : IWeapon
{
    public readonly Entity owner;

    protected float lastTriggerTime;
    protected float currentOverCharge;
    protected Vector2 direction = Vector2.up;
    
    public abstract float ColdDown { get; }
    public abstract float HitWeight { get; }
    public abstract float OverChargeValue { get; }

    public Weapon (Entity owner)
    {
        this.owner = owner;
    }

    public bool CanUse()
    {
        float time = Time.time - lastTriggerTime;
        return time >= ColdDown;
    }

    public abstract void OverCharge();
    public abstract void Shoot();

    public void Trigger(Vector2 direction)
    {
        if (!CanUse()) return;

        this.direction = direction.normalized;

        if (currentOverCharge > OverChargeValue) {
            OverCharge();
            currentOverCharge = 0.0f;
        }
        else
        {
            Shoot();
            currentOverCharge += Random.Range(8.0f, 11.0f);
        }

        lastTriggerTime = Time.time;
    }
}
