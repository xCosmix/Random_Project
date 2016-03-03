using UnityEngine;
using System.Collections;
using BehaviourTree;
using EnemyActions;

public class enemyTest : Enemy {

	// Use this for initialization
	protected override void EnemyStart () {
        Node root = new Priority
        (
           new Node[]
           {
                new Sequence
                (
                    new Node[]
                    {
                        new ButtonPressed("X"),
                        new MemSequence
                        (
                            new Node[]
                            {
                                new ChangeColor(Color.blue),
                                new Wait(1.0f),
                                new XAction<RandomMovement>(5.0f, default_forces[0])
                               // new ChangeColor(Color.red)
                            }
                        )
                    }
                ),
                new ChangeColor(Color.red)
           }
        );
        behaviourTree = new BehaviourTree.Tree(root);
	}

}
