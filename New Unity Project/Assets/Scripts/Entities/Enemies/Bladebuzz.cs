using UnityEngine;
using System.Collections;
using BehaviourTree;
using EnemyActions;
using EnemyNodes;

public class Bladebuzz : Enemy {

    public override float CastTime
    {
        get { return 0.5f; }
    }

    protected override void EnemyStart()
    {

        weapon = new NormalBlade(this);

        Node root = new MemPriority
        (
           new Node[]
           {
                new MemSequence
                (
                    new Node[]
                    {
                        new TargetOnSight(fieldOfView),
                        new WillShoot(3.0f, 8.0f, 30),
                        new TimeLimit 
                        (
                            3.0f,
                            new XAction<Aproach>(0.5f, 1.5f, -10.0f, 10.0f)
                        ),
                        new MemSequence
                        (
                            new Node[]
                            {
                                new XAction<CastAttack>(),
                                new XAction<Aim>(0.8f),
                                new XAction<Shoot>()
                            }
                        )
                    }
                ),
                new MemProbabilistic
                (
                    new Node[]
                    {
                        new MemSequence
                        (
                            new Node[]
                            {
                                new WaitRandom(0.3f, 1.5f),
                                new XAction<RandomMovement>(4.0f)
                            }
                        ),
                        new MemSequence
                        (
                            new Node[]
                            {
                                new WaitRandom(0.3f, 1.5f)
                            }
                        ),
                    }
                ),
           }
        );

        behaviourTree = new BehaviourTree.Tree(root);
    }
}
