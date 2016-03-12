using UnityEngine;
using BehaviourTree;
using EnemyActions;
using EnemyNodes;
using System.Collections;
using System;

public class Blastbark : Enemy {

    public override float CastTime
    {
        get { return 1.0f; }
    }

    protected override void EnemyStart()
    {

        weapon = new NormalCannon(this);

        Node root = new MemPriority
        (
           new Node[]
           {
                new MemSequence
                (
                        new Node[]
                        {
                            new TargetOnSight(fieldOfView),
                            new WillShoot(5.0f, 10.0f, 30),
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
                                new WaitRandom(0.2f, 1.0f),
                                new XAction<RandomMovement>(5.0f)
                            }
                        ),
                        new MemSequence
                        (
                            new Node[]
                            {
                                new WaitRandom(0.7f, 2.0f)
                            }
                        ),
                    }
                ),
           }
        );

        behaviourTree = new BehaviourTree.Tree(root);
    }

}
