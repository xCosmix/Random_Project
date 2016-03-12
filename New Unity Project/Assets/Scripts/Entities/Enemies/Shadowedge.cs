using UnityEngine;
using System.Collections;
using BehaviourTree;
using EnemyActions;
using EnemyNodes;

public class Shadowedge : Enemy
{

    public override float CastTime
    {
        get { return 0.5f; }
    }

    protected override void EnemyStart()
    {

        weapon = new NormalBlade(this);

        Node root = 
        new Priority 
        ( 
            new Node[]
            {
                new MemSequence 
                (
                    new Node[]
                    {
                        new Probability(20, 0.2f),
                        new XAction<Dodge>(50.0f, 2.0f, 1.0f)
                    }
                )
                ,
                new MemPriority
                (
                new Node[]
                {
                    new MemSequence
                    (
                        new Node[]
                        {
                            new TargetOnSight(fieldOfView),
                            new Probability(50, 0.5f),
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
                                    new WaitRandom(0.2f, 1.0f),
                                    new XAction<RandomMovement>(4.0f)
                                }
                            ),
                            new MemSequence
                            (
                                new Node[]
                                {
                                    new WaitRandom(0.3f, 1.0f)
                                }
                            ),
                        }
                    ),
                }
            )
        }
        );

        behaviourTree = new BehaviourTree.Tree(root);
    }
}
