using UnityEngine;
using System.Collections;
using BehaviourTree;
using EnemyActions;

public class enemyTest : Enemy {

	// Use this for initialization
	protected override void EnemyStart () {

        weapon = new NormalCannon(this);

        Node root = new Priority
        (
           new Node[]
           {
                new XProbabilistic
                (
                    new Node[]
                    {
                        new MemSequence
                        (
                            new Node[]
                            {
                                //new ChangeColor(Color.blue),
                                new WaitRandom(0.2f, 1.0f),
                                new XAction<RandomMovement>(5.0f)
                            }
                        ),
                        new MemSequence 
                        (
                            new Node[]
                            {
                                //new ChangeColor(Color.red),
                                new WaitRandom(0.7f, 3.0f)
                            }
                        ),
                        new MemSequence
                        (
                            new Node[]
                            {
                                new XAction<Aim>(0.8f),
                                new XAction<Shoot>()
                            }
                        ),
                    }
                ),
           }
        );
        behaviourTree = new BehaviourTree.Tree(root);
	}

}
