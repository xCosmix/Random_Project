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
        weapon = new ContactHazard(this, 2.0f, 0.2f);

        Node root = new MemProbabilistic
        (
           new Node[]
           {
                new MemSequence
                (
                    new Node[]
                    {
                        new TimeLimit
                        (
                            10.0f,
                            new XAction<Follow>()
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

    protected override void EnemyUpdate()
    {
        weapon.Trigger(Vector2.zero);
    }
}
