using UnityEngine;
using System.Collections;
using BehaviourTree;

public class Enemy : Entity {

    protected BehaviourTree.Tree behaviourTree;
    protected Blackboard blackBoard;

    public float fieldOfView = 10.0f;

    protected override void CustomStart()
    {
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
}
