using UnityEngine;
using System.Collections;
using BehaviourTree;
using ObjectPool;

public abstract class Enemy : Entity {

    protected static GameObject energyPointPrefab;

    protected BehaviourTree.Tree behaviourTree;
    protected Blackboard blackBoard;

    public float fieldOfView = 50.0f;

    [Range(5, 1000)]
    public int scoreValue = 30;

    public Weapon weapon;
    protected float castTime;
    public abstract float CastTime
    {
        get;
    }

    protected Vector2 aimDir;
    public override Vector2 AimDir
    {
        get { return aimDir; }
        set { aimDir = value; }
    }

    protected override void CustomStart()
    {
        animator = GetComponent<Animator>();
        energyPointPrefab = Resources.Load<GameObject>("Prefabs/EnergyDrop");
        blackBoard = new Blackboard(this.gameObject);
        EnemyStart();
    }
    protected virtual void EnemyStart ()
    {

    }

    protected override void CustomUpdate()
    {
        EnemyUpdate();
        behaviourTree.Tick(blackBoard);
    }
    protected virtual void EnemyUpdate()
    {

    }
    protected override IEnumerator DieAnimation()
    {
        yield return null;
        DieDrop();        
    }
    protected void DieDrop ()
    {
        if (Player.player.stats.life < 2)
        {
            Player.player.stats.life++;
            return;
        }
        float biggerMount = (float)scoreValue / (float)EnergyPoint.maxValue;

        if (biggerMount <= 1.0f)
        {
            GameObject drop = Pool.New(energyPointPrefab, transform.position, energyPointPrefab.transform.rotation, 50);
            EnergyPoint dropEnergy = drop.GetComponent<EnergyPoint>();
            dropEnergy.New(Mathf.RoundToInt(scoreValue * Random.Range(0.8f, 1.2f)));
        }
        else
        {
            GameObject drop;
            EnergyPoint dropEnergy;
            float decimalDifference = Mathf.FloorToInt(biggerMount) - biggerMount;
            int lastDropValue = Mathf.RoundToInt(decimalDifference * EnergyPoint.maxValue);
            for (int i = 0; i < biggerMount-1; i++)
            {
                drop = Pool.New(energyPointPrefab, transform.position, energyPointPrefab.transform.rotation, 50);
                dropEnergy = drop.GetComponent<EnergyPoint>();
                dropEnergy.New(Mathf.RoundToInt(EnergyPoint.maxValue * Random.Range(0.8f, 1.2f)));
            }

            drop = Pool.New(energyPointPrefab, transform.position, energyPointPrefab.transform.rotation, 50);
            dropEnergy = drop.GetComponent<EnergyPoint>();
            dropEnergy.New(Mathf.RoundToInt(lastDropValue * Random.Range(0.8f, 1.2f)));
        }
    }
}
