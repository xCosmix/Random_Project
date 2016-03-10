using UnityEngine;
using System.Collections;
using BehaviourTree;
using EnemyActions;
using EnemyNodes;

public class Dod : Enemy {

    public override float CastTime
    {
        get { return 1.0f; }
    }

    protected override void EnemyStart()
    {

        weapon = new NormalBlade(this);

        Node root = new MemProbabilistic
        (
           new Node[]
           {
                new MemSequence
                (
                    new Node[]
                    {
                        new XAction<Follow>(12.0f),
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
                                new XAction<RandomMovement>(6.0f)
                            }
                        ),
                        new MemSequence
                        (
                            new Node[]
                            {
                                new WaitRandom(0.3f, 0.6f)
                            }
                        ),
                    }
                ),
           }
        );

        behaviourTree = new BehaviourTree.Tree(root);
    }
}
